using AutoMapper;
using HRM.Application.DTOs.Department;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using MediatR;

namespace HRM.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler(IDepartmentRepository repository, IMapper mapper)
        : IRequestHandler<CreateDepartmentCommand, DepartmentResponseDto>
{
    public async Task<DepartmentResponseDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await repository.NameExistsAsync(request.Dto.Name);
        if (nameExists)
            throw new InvalidOperationException($"A department with name '{request.Dto.Name}' already exists.");

        var department = mapper.Map<Department>(request.Dto);
        var created = await repository.CreateAsync(department);

        return mapper.Map<DepartmentResponseDto>(created);
    }
}
