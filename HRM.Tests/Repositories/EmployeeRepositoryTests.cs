using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using HRM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRM.Tests.Repositories;

public class EmployeeRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Employee MakeEmployee(string email = "john@company.com") => new()
    {
        FirstName = "John",
        LastName = "Doe",
        Email = email,
        JobTitle = "Engineer",
        Salary = 5000,
        HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
    };

    [Fact]
    public async Task EmailExistsAsync_ActiveEmployeeWithEmail_ReturnsTrue()
    {
        using var ctx = CreateContext();
        var repo = new EmployeeRepository(ctx);
        await repo.CreateAsync(MakeEmployee("exists@company.com"));

        var result = await repo.EmailExistsAsync("exists@company.com");

        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_NoEmployeeWithEmail_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new EmployeeRepository(ctx);

        var result = await repo.EmailExistsAsync("nobody@company.com");

        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_InactiveEmployeeWithEmail_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new EmployeeRepository(ctx);
        var employee = MakeEmployee("inactive@company.com");
        await repo.CreateAsync(employee);
        await repo.DeactivateAsync(employee.Id);

        var result = await repo.EmailExistsAsync("inactive@company.com");

        Assert.False(result);
    }
}
