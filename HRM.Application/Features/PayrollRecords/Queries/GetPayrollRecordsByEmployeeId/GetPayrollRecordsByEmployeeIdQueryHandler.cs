using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordsByEmployeeId;

public class GetPayrollRecordsByEmployeeIdQueryHandler(IPayrollRepository repository, IMapper mapper)
    : IRequestHandler<GetPayrollRecordsByEmployeeIdQuery, PagedResult<PayrollRecordResponseDto>>
{
    public async Task<PagedResult<PayrollRecordResponseDto>> Handle(GetPayrollRecordsByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var pagedPayrollRecords = await repository.GetByEmployeeIdAsync(request.EmployeeId, request.PageNumber, request.PageSize);
        return new PagedResult<PayrollRecordResponseDto>
        {
            Items = mapper.Map<IEnumerable<PayrollRecordResponseDto>>(pagedPayrollRecords.Items),
            TotalCount = pagedPayrollRecords.TotalCount,
            PageNumber = pagedPayrollRecords.PageNumber,
            PageSize = pagedPayrollRecords.PageSize
        };
    }
}
