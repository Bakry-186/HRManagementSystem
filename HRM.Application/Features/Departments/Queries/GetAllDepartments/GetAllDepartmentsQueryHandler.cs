using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.Department;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Departments.Queries.GetAllDepartments;

public class GetAllDepartmentsQueryHandler(IDepartmentRepository repository, IMapper mapper)
        : IRequestHandler<GetAllDepartmentsQuery, PagedResult<DepartmentResponseDto>>
{
    public async Task<PagedResult<DepartmentResponseDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var pagedDepartments = await repository.GetAllAsync(request.PageNumber, request.PageSize);

        return new PagedResult<DepartmentResponseDto>
        {
            Items = mapper.Map<IEnumerable<DepartmentResponseDto>>(pagedDepartments.Items),
            TotalCount = pagedDepartments.TotalCount,
            PageNumber = pagedDepartments.PageNumber,
            PageSize = pagedDepartments.PageSize
        };
    }
}
