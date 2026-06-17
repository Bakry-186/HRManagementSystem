using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler(IEmployeeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllEmployeesQuery, PagedResult<EmployeeResponseDto>>
{
    public async Task<PagedResult<EmployeeResponseDto>> Handle(
        GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var pagedEmployees = await repository.GetAllAsync(request.PageNumber, request.PageSize);

        return new PagedResult<EmployeeResponseDto>
        {
            Items = mapper.Map<IEnumerable<EmployeeResponseDto>>(pagedEmployees.Items),
            TotalCount = pagedEmployees.TotalCount,
            PageNumber = pagedEmployees.PageNumber,
            PageSize = pagedEmployees.PageSize
        };
    }
}
