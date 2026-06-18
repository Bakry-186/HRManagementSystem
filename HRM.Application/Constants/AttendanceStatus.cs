namespace HRM.Application.Constants;

public static class AttendanceStatus
{
    public const string Present = "Present";
    public const string Absent = "Absent";
    public const string Late = "Late";
    public const string HalfDay = "HalfDay";

    public static readonly string[] All = [Present, Absent, Late, HalfDay];
}
