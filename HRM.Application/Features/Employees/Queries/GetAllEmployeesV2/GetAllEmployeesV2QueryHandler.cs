using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetAllEmployeesV2;

public class GetAllEmployeesV2QueryHandler(IEmployeeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllEmployeesV2Query, PagedResult<EmployeeV2ResponseDto>>
{
    public async Task<PagedResult<EmployeeV2ResponseDto>> Handle(
        GetAllEmployeesV2Query request, CancellationToken cancellationToken)
    {
        var pagedEmployees = await repository.GetAllAsync(request.PageNumber, request.PageSize);

        return new PagedResult<EmployeeV2ResponseDto>
        {
            Items = mapper.Map<IEnumerable<EmployeeV2ResponseDto>>(pagedEmployees.Items),
            TotalCount = pagedEmployees.TotalCount,
            PageNumber = pagedEmployees.PageNumber,
            PageSize = pagedEmployees.PageSize
        };
    }
}
