using FluentValidation;
using HRM.Application.DTOs.PayrollRecord;

namespace HRM.Application.Validators.Payroll;

public class UpdatePayrollValidator : AbstractValidator<UpdatePayrollRecordDto>
{
    public UpdatePayrollValidator()
    {
        RuleFor(x => x.PeriodStart)
            .NotEmpty().WithMessage("Period start is required.")
            .LessThan(x => x.PeriodEnd).WithMessage("Period start must be before period end.");

        RuleFor(x => x.PeriodEnd)
            .NotEmpty().WithMessage("Period end is required.");

        RuleFor(x => x.BasicSalary)
            .GreaterThan(0).WithMessage("Basic salary must be greater than zero.");

        RuleFor(x => x.OvertimeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Overtime amount cannot be negative.");

        RuleFor(x => x.Bonus)
            .GreaterThanOrEqualTo(0).WithMessage("Bonus cannot be negative.");

        RuleFor(x => x.Deductions)
            .GreaterThanOrEqualTo(0).WithMessage("Deductions cannot be negative.");
    }
}
