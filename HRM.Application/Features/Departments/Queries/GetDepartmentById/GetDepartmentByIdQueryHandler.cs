using AutoMapper;
using HRM.Application.DTOs.Department;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQueryHandler(IDepartmentRepository repository, IMapper mapper)
        : IRequestHandler<GetDepartmentByIdQuery, DepartmentResponseDto>
{
    public async Task<DepartmentResponseDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Department with ID {request.Id} was not found.");

        return mapper.Map<DepartmentResponseDto>(department);
    }
}
