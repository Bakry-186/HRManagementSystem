using System.Security.Claims;
using HRM.Domain.Entities;
using HRM.Infrastructure.Interceptors;
using HRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace HRM.Tests.Interceptors;

public class AuditInterceptorTests
{
    private static AppDbContext CreateContext(AuditInterceptor interceptor)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static AuditInterceptor CreateInterceptor(string? username = "admin")
    {
        var httpContextMock = new Mock<IHttpContextAccessor>();

        if (username is not null)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            httpContextMock.Setup(x => x.HttpContext).Returns(httpContext);
        }
        else
        {
            httpContextMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        }

        return new AuditInterceptor(httpContextMock.Object);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Create entity → Action = "Created", OldValues = null
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task SaveChanges_OnCreate_GeneratesAuditLogWithCreatedAction()
    {
        var interceptor = CreateInterceptor("admin");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe",
            Email = "john@example.com", JobTitle = "Dev",
            HireDate = new DateOnly(2024, 1, 1), Salary = 5000
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var log = context.AuditLogs.SingleOrDefault(a => a.EntityId == employee.Id);

        Assert.NotNull(log);
        Assert.Equal("Created", log.Action);
        Assert.Equal("Employee", log.EntityName);
        Assert.Equal(employee.Id, log.EntityId);
        Assert.Null(log.OldValues);
    }

    [Fact]
    public async Task SaveChanges_OnCreate_NewValuesIsNotEmpty()
    {
        var interceptor = CreateInterceptor("admin");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith",
            Email = "jane@example.com", JobTitle = "QA",
            HireDate = new DateOnly(2024, 1, 1), Salary = 4000
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var log = context.AuditLogs.Single(a => a.EntityId == employee.Id);

        Assert.NotEmpty(log.NewValues);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Update entity → Action = "Updated", OldValues populated
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task SaveChanges_OnUpdate_GeneratesAuditLogWithUpdatedAction()
    {
        var interceptor = CreateInterceptor("admin");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Brown",
            Email = "alice@example.com", JobTitle = "PM",
            HireDate = new DateOnly(2024, 1, 1), Salary = 6000
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        employee.JobTitle = "Senior PM";
        await context.SaveChangesAsync();

        var updateLog = context.AuditLogs
            .Where(a => a.EntityId == employee.Id && a.Action == "Updated")
            .SingleOrDefault();

        Assert.NotNull(updateLog);
        Assert.Equal("Updated", updateLog.Action);
        Assert.NotNull(updateLog.OldValues);
    }

    [Fact]
    public async Task SaveChanges_OnUpdate_OldValuesContainsPreviousData()
    {
        var interceptor = CreateInterceptor("admin");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Bob", LastName = "King",
            Email = "bob@example.com", JobTitle = "Analyst",
            HireDate = new DateOnly(2024, 1, 1), Salary = 5500
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        employee.JobTitle = "Senior Analyst";
        await context.SaveChangesAsync();

        var updateLog = context.AuditLogs
            .Single(a => a.EntityId == employee.Id && a.Action == "Updated");

        Assert.Contains("Analyst", updateLog.OldValues);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ChangedBy captured from JWT claim
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task SaveChanges_WithAuthenticatedUser_SetsChangedBy()
    {
        var interceptor = CreateInterceptor("hr_manager");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Carol", LastName = "White",
            Email = "carol@example.com", JobTitle = "Designer",
            HireDate = new DateOnly(2024, 1, 1), Salary = 4500
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var log = context.AuditLogs.Single(a => a.EntityId == employee.Id);

        Assert.Equal("hr_manager", log.ChangedBy);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // No HTTP context (background job) → ChangedBy = "System"
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task SaveChanges_WithoutHttpContext_SetsChangedByToSystem()
    {
        var interceptor = CreateInterceptor(username: null);
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Dave", LastName = "Gray",
            Email = "dave@example.com", JobTitle = "Ops",
            HireDate = new DateOnly(2024, 1, 1), Salary = 4800
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var log = context.AuditLogs.Single(a => a.EntityId == employee.Id);

        Assert.Equal("System", log.ChangedBy);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AuditLog entries themselves are not audited — no infinite recursion
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task SaveChanges_AuditLogEntriesAreNotAudited()
    {
        var interceptor = CreateInterceptor("admin");
        await using var context = CreateContext(interceptor);

        var employee = new Employee
        {
            Id = Guid.NewGuid(), FirstName = "Eve", LastName = "Clark",
            Email = "eve@example.com", JobTitle = "Lead",
            HireDate = new DateOnly(2024, 1, 1), Salary = 7000
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        // Only one AuditLog should exist — for the Employee, not for the AuditLog itself
        var auditCount = context.AuditLogs.Count(a => a.EntityId == employee.Id);

        Assert.Equal(1, auditCount);
        Assert.Equal(1, context.AuditLogs.Count());
    }
}
