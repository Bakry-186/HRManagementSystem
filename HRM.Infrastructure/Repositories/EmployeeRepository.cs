using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context)
    : GenericRepository<Employee>(context), IEmployeeRepository
{
    public Task<bool> EmailExistsAsync(string email)
    {
        return DbSet.AnyAsync(e => e.Email == email && e.IsActive);
    }
}
