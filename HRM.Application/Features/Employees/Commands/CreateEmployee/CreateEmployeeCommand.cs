using HRM.Application.DTOs.Employee;
using MediatR;

namespace HRM.Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeDto Dto) : IRequest<EmployeeResponseDto>;
