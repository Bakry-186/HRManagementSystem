using HRM.Domain.Entities;
namespace HRM.Application.Interfaces;

public interface IAttendanceRepository : IGenericRepository<AttendanceRecord>
{
    Task<AttendanceRecord?> GetByEmployeeIdAndDateAsync(Guid employeeId, DateOnly date);
}
