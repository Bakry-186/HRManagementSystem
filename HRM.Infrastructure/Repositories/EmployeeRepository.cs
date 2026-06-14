using HRM.Application.Common.Models;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        return employee;
    }

    public async Task<PagedResult<Employee>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = context.Employees.Where(e => e.IsActive);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Employee>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        employee.Id = Guid.NewGuid();
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee is null) return false;

        employee.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return context.Employees.AnyAsync(e => e.Email == email);
    }
}
