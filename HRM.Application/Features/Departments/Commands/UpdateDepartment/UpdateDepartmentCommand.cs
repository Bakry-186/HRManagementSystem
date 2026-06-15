using HRM.Application.DTOs.Department;
using MediatR;

namespace HRM.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(Guid Id, CreateOrUpdateDepartmentDto Dto) : IRequest<DepartmentResponseDto>;
