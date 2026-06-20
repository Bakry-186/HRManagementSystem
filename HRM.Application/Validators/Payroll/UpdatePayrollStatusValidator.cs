using FluentValidation;
using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;

namespace HRM.Application.Validators.Payroll;

public class UpdatePayrollStatusValidator : AbstractValidator<UpdatePayrollRecordStatusDto>
{
    public UpdatePayrollStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => PayrollStatus.All.Contains(s)).WithMessage($"Status must be one of: {string.Join(", ", PayrollStatus.All)}.");
    }
}
