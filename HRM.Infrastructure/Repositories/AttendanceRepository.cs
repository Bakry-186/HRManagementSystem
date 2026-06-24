using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class AttendanceRepository(AppDbContext context) : GenericRepository<AttendanceRecord>(context), IAttendanceRepository
{
    public async Task<AttendanceRecord?> GetByEmployeeIdAndDateAsync(Guid employeeId, DateOnly date)
    {
        return await DbSet.FirstOrDefaultAsync(ar => ar.EmployeeId == employeeId && ar.Date == date && ar.IsActive);
    }

    public async Task<List<AttendanceRecord>> GetByEmployeeIdAndPeriodAsync(Guid employeeId, DateOnly periodStart, DateOnly periodEnd)
    {
        return await DbSet
            .Where(ar => ar.EmployeeId == employeeId && ar.Date >= periodStart && ar.Date <= periodEnd && ar.IsActive)
            .OrderBy(ar => ar.Date)
            .ToListAsync();
    }
}
