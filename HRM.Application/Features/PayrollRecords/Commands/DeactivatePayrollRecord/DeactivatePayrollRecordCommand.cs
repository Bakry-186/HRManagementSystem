using MediatR;
namespace HRM.Application.Features.PayrollRecords.Commands.DeactivatePayrollRecord;

public record DeactivatePayrollRecordCommand(Guid Id) : IRequest<bool>;
