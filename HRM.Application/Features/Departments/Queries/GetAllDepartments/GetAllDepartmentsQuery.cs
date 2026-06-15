using HRM.Application.Common.Models;
using HRM.Application.DTOs.Department;
using MediatR;

namespace HRM.Application.Features.Departments.Queries.GetAllDepartments;

public record GetAllDepartmentsQuery(int PageNumber = 1, int PageSize = 10)
        : IRequest<PagedResult<DepartmentResponseDto>>;
