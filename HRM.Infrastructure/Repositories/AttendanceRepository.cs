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
}
