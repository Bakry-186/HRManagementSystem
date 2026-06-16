using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using HRM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRM.Tests.Repositories;

public class DepartmentRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task NameExistsAsync_ActiveDepartmentWithName_ReturnsTrue()
    {
        using var ctx = CreateContext();
        var repo = new DepartmentRepository(ctx);
        await repo.CreateAsync(new Department { Name = "Engineering" });

        var result = await repo.NameExistsAsync("Engineering");

        Assert.True(result);
    }

    [Fact]
    public async Task NameExistsAsync_NoDepartmentWithName_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new DepartmentRepository(ctx);

        var result = await repo.NameExistsAsync("Finance");

        Assert.False(result);
    }

    [Fact]
    public async Task NameExistsAsync_InactiveDepartmentWithName_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new DepartmentRepository(ctx);
        var dept = await repo.CreateAsync(new Department { Name = "Legal" });
        await repo.DeactivateAsync(dept.Id);

        var result = await repo.NameExistsAsync("Legal");

        Assert.False(result);
    }
}
