using HRM.Application.DTOs.Department;
using MediatR;

namespace HRM.Application.Features.Departments.Queries.GetDepartmentById;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentResponseDto>;
