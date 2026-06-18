namespace HRM.Application.DTOs.AttendanceRecord;

public class UpdateAttendanceRecordDto
{
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
