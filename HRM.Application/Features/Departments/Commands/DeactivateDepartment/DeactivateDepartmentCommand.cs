using MediatR;

namespace HRM.Application.Features.Departments.Commands.DeactivateDepartment;

public record DeactivateDepartmentCommand(Guid Id) : IRequest<bool>;
