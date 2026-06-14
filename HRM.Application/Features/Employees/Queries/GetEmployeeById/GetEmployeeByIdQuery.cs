using HRM.Application.DTOs.Employee;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeResponseDto>;
