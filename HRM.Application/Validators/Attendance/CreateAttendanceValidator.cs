using FluentValidation;
using HRM.Application.Constants;
using HRM.Application.DTOs.AttendanceRecord;

namespace HRM.Application.Validators.Attendance;

public class CreateAttendanceValidator : AbstractValidator<CreateAttendanceRecordDto>
{
    public CreateAttendanceValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty()
            .WithMessage("Employee ID is required.");
        RuleFor(x => x.Date).NotEmpty()
            .WithMessage("Date is required.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future.");
        RuleFor(x => x.CheckInTime).NotEmpty()
            .WithMessage("Check-in time is required.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => AttendanceStatus.All.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AttendanceStatus.All)}.");

        RuleFor(x => x.CheckOutTime)
            .Must((dto, checkOut) => !checkOut.HasValue || checkOut.Value > dto.CheckInTime)
            .WithMessage("Check-out time must be after check-in time.");
    }
}
