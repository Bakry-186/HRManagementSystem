using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using HRM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRM.Tests.Repositories;

public class GenericRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Department MakeDepartment(string name = "Engineering") => new()
    {
        Name = name,
        Description = "Test department"
    };

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ExistingActiveEntity_ReturnsEntity()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        var result = await repo.GetByIdAsync(dept.Id);

        Assert.NotNull(result);
        Assert.Equal(dept.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_InactiveEntity_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);
        await repo.DeactivateAsync(dept.Id);

        var result = await repo.GetByIdAsync(dept.Id);

        Assert.Null(result);
    }

    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveEntities()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var active = MakeDepartment("HR");
        var inactive = MakeDepartment("Legal");
        await repo.CreateAsync(active);
        await repo.CreateAsync(inactive);
        await repo.DeactivateAsync(inactive.Id);

        var result = await repo.GetAllAsync(1, 10);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCorrectTotalCount()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        await repo.CreateAsync(MakeDepartment("A"));
        await repo.CreateAsync(MakeDepartment("B"));
        await repo.CreateAsync(MakeDepartment("C"));

        var result = await repo.GetAllAsync(1, 10);

        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCorrectPaginationMetadata()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        await repo.CreateAsync(MakeDepartment("A"));
        await repo.CreateAsync(MakeDepartment("B"));
        await repo.CreateAsync(MakeDepartment("C"));

        var result = await repo.GetAllAsync(2, 2);

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_EmptyRepository_ReturnsEmptyResult()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);

        var result = await repo.GetAllAsync(1, 10);

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    // CreateAsync

    [Fact]
    public async Task CreateAsync_AssignsNonEmptyId()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();

        await repo.CreateAsync(dept);

        Assert.NotEqual(Guid.Empty, dept.Id);
    }

    [Fact]
    public async Task CreateAsync_PersistsEntityInDatabase()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();

        await repo.CreateAsync(dept);

        var fromDb = await ctx.Departments.FindAsync(dept.Id);
        Assert.NotNull(fromDb);
    }

    // UpdateAsync

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        dept.Name = "Updated Name";
        await repo.UpdateAsync(dept);

        var fromDb = await ctx.Departments.FindAsync(dept.Id);
        Assert.Equal("Updated Name", fromDb!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedEntity()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        dept.Description = "New description";
        var result = await repo.UpdateAsync(dept);

        Assert.Equal("New description", result.Description);
    }

    // DeactivateAsync

    [Fact]
    public async Task DeactivateAsync_ExistingEntity_ReturnsTrue()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        var result = await repo.DeactivateAsync(dept.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeactivateAsync_SetsIsActiveToFalse()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        await repo.DeactivateAsync(dept.Id);

        var fromDb = await ctx.Departments.FindAsync(dept.Id);
        Assert.False(fromDb!.IsActive);
    }

    [Fact]
    public async Task DeactivateAsync_NonExistingEntity_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);

        var result = await repo.DeactivateAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeactivateAsync_DeactivatedEntity_NotReturnedByGetById()
    {
        using var ctx = CreateContext();
        var repo = new GenericRepository<Department>(ctx);
        var dept = MakeDepartment();
        await repo.CreateAsync(dept);

        await repo.DeactivateAsync(dept.Id);
        var result = await repo.GetByIdAsync(dept.Id);

        Assert.Null(result);
    }
}
