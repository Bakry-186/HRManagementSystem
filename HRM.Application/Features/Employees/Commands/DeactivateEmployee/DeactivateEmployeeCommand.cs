using MediatR;

namespace HRM.Application.Features.Employees.Commands.DeactivateEmployee;

public record DeactivateEmployeeCommand(Guid Id) : IRequest<bool>;
