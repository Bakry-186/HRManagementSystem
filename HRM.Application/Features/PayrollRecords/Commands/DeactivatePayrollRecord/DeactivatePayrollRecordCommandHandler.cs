using HRM.Application.Interfaces;
using MediatR;
namespace HRM.Application.Features.PayrollRecords.Commands.DeactivatePayrollRecord;

public class DeactivatePayrollRecordCommandHandler(IPayrollRepository repository)
    : IRequestHandler<DeactivatePayrollRecordCommand, bool>
{
    public async Task<bool> Handle(DeactivatePayrollRecordCommand request, CancellationToken cancellationToken)
    {
        var deactivated = await repository.DeactivateAsync(request.Id);
        if (!deactivated)
            throw new KeyNotFoundException($"Payroll record with ID {request.Id} was not found.");
        return true;
    }

}
