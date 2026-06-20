using HRM.Domain.Entities;
namespace HRM.Application.Interfaces;

public interface IAttendanceRepository : IGenericRepository<AttendanceRecord>
{
    Task<AttendanceRecord?> GetByEmployeeIdAndDateAsync(Guid employeeId, DateOnly date);
    Task<List<AttendanceRecord>> GetByEmployeeIdAndPeriodAsync(Guid employeeId, DateOnly periodStart, DateOnly periodEnd);
}
