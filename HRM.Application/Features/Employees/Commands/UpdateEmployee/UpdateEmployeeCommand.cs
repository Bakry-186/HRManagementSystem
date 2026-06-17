using HRM.Application.DTOs.Employee;
using MediatR;

namespace HRM.Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeDto Dto) : IRequest<EmployeeResponseDto>;
