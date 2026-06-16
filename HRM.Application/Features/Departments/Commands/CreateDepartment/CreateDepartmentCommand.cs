using HRM.Application.DTOs.Department;
using MediatR;

namespace HRM.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentDto Dto) : IRequest<DepartmentResponseDto>;
