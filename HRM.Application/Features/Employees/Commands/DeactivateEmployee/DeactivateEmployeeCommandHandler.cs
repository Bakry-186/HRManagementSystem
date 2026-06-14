using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Employees.Commands.DeactivateEmployee;

public class DeactivateEmployeeCommandHandler(IEmployeeRepository repository)
    : IRequestHandler<DeactivateEmployeeCommand, bool>
{
    public async Task<bool> Handle(
        DeactivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var deactivated = await repository.DeactivateAsync(request.Id);
        if (!deactivated)
            throw new KeyNotFoundException($"Employee with ID {request.Id} was not found.");

        return true;
    }
}
