using HRM.Application.Common.Models;
using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<PagedResult<Employee>> GetAllAsync(int pageNumber, int pageSize);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
}
