namespace HRM.Application.DTOs.AttendanceRecord;

public class CreateAttendanceRecordDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
