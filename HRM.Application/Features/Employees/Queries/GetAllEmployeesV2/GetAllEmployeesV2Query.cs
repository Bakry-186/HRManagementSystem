using HRM.Application.Common.Models;
using HRM.Application.DTOs.Employee;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetAllEmployeesV2;

public record GetAllEmployeesV2Query(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<EmployeeV2ResponseDto>>;
