using HRM.Application.Common.Models;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class PayrollRepository(AppDbContext context) : GenericRepository<PayrollRecord>(context), IPayrollRepository
{
    public async Task<PayrollRecord?> GetByEmployeeIdAndPeriodAsync(Guid employeeId, DateOnly periodStart)
    {
        return await DbSet.FirstOrDefaultAsync(pr => pr.EmployeeId == employeeId && pr.PeriodStart == periodStart && pr.IsActive);
    }

    public async Task<PagedResult<PayrollRecord>> GetByEmployeeIdAsync(Guid employeeId, int pageNumber, int pageSize)
    {
        var query = DbSet.Where(pr => pr.EmployeeId == employeeId && pr.IsActive);
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(pr => pr.PeriodStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedResult<PayrollRecord>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        var payrollRecord = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Payroll record with ID {id} was not found.");
        payrollRecord.Status = status;
        var updated = await UpdateAsync(payrollRecord);
        return updated != null;
    }

}
