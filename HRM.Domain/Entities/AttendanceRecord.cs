namespace HRM.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public required Guid EmployeeId { get; set; }
    public required DateOnly Date { get; set; }
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public required string Status { get; set; }
    public string? Notes { get; set; }
}
