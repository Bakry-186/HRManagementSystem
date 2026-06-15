using HRM.Application.Common.Models;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext context) : IDepartmentRepository
{
    public async Task<Department> CreateAsync(Department department)
    {
        department.Id = Guid.NewGuid();
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        return department;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var department = await context.Departments.FindAsync(id);
        if (department is null) return false;

        department.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<Department>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = context.Departments.Where(d => d.IsActive);
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Department>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        var department = await context.Departments.FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        return department;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Departments.AnyAsync(d => d.Name == name && d.IsActive);
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        context.Departments.Update(department);
        await context.SaveChangesAsync();
        return department;
    }
}
