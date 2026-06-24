using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Departments.Commands.DeactivateDepartment;

public class DeactivateDepartmentCommandHandler(IDepartmentRepository repository)
    : IRequestHandler<DeactivateDepartmentCommand, bool>
{
    public async Task<bool> Handle(DeactivateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var deactivated = await repository.DeactivateAsync(request.Id);
        if (!deactivated)
            throw new KeyNotFoundException($"Department with ID {request.Id} was not found.");

        return true;
    }
}
