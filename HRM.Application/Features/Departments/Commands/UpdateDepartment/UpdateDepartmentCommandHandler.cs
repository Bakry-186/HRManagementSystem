using AutoMapper;
using HRM.Application.DTOs.Department;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler(IDepartmentRepository repository, IMapper mapper)
    : IRequestHandler<UpdateDepartmentCommand, DepartmentResponseDto>
{
    public async Task<DepartmentResponseDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Department with ID {request.Id} was not found.");

        mapper.Map(request.Dto, department);
        var updated = await repository.UpdateAsync(department);

        return mapper.Map<DepartmentResponseDto>(updated);

    }
}
