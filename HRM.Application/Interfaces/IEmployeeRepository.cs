using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<bool> EmailExistsAsync(string email);
}
