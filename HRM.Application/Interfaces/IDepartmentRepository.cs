using HRM.Application.Common.Models;
using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id);
    Task<PagedResult<Department>> GetAllAsync(int pageNumber, int pageSize);
    Task<Department> CreateAsync(Department department);
    Task<Department> UpdateAsync(Department department);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
}
