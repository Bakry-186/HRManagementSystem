using FluentValidation;
using HRM.Application.Constants;
using HRM.Application.DTOs.AttendanceRecord;

namespace HRM.Application.Validators.Attendance;

public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceRecordDto>
{
    public UpdateAttendanceValidator()
    {
        RuleFor(x => x.CheckOutTime)
            .Must((dto, checkOut) => !checkOut.HasValue || !dto.CheckInTime.HasValue || checkOut.Value > dto.CheckInTime!.Value)
            .WithMessage("Check-out time must be after check-in time.");
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(s => AttendanceStatus.All.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AttendanceStatus.All)}.");
    }
}
