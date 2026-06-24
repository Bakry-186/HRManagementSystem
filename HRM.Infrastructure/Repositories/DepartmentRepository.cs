using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext context)
    : GenericRepository<Department>(context), IDepartmentRepository
{
    public Task<bool> NameExistsAsync(string name)
    {
        return DbSet.AnyAsync(d => d.Name == name && d.IsActive);
    }
}
