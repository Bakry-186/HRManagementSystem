using HRM.Application.Common.Models;
using HRM.Application.DTOs.Employee;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetAllEmployees;

public record GetAllEmployeesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<EmployeeResponseDto>>;
