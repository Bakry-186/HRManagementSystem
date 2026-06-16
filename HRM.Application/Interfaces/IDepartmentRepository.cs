using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<bool> NameExistsAsync(string name);
}
