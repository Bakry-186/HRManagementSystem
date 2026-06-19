using HRM.Application.Common.Models;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Domain.Entities;
namespace HRM.Application.Interfaces;

public interface IPayrollRepository : IGenericRepository<PayrollRecord>
{
    Task<PayrollRecord?> GetByEmployeeIdAndPeriodAsync(Guid employeeId, DateOnly periodStart);
    Task<PagedResult<PayrollRecord>> GetByEmployeeIdAsync(Guid employeeId, int pageNumber, int pageSize);
    Task<bool> UpdateStatusAsync(Guid id, string status);
}
