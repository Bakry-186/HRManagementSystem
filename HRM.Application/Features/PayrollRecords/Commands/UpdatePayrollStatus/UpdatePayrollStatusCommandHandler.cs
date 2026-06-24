using HRM.Application.Interfaces;
using MediatR;
namespace HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollStatus;

public class UpdatePayrollStatusCommandHandler(IPayrollRepository repository)
    : IRequestHandler<UpdatePayrollStatusCommand, bool>
{
    public async Task<bool> Handle(UpdatePayrollStatusCommand request, CancellationToken cancellationToken)
    {
        return await repository.UpdateStatusAsync(request.Id, request.Dto.Status);
    }
}
