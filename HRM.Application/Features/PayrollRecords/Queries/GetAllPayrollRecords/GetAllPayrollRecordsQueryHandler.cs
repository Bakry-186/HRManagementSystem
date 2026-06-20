using AutoMapper;
using MediatR;

using HRM.Application.Interfaces;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.PayrollRecord;
namespace HRM.Application.Features.PayrollRecords.Queries.GetAllPayrollRecords;

public class GetAllPayrollRecordsQueryHandler(IPayrollRepository repository, IMapper mapper)
    : IRequestHandler<GetAllPayrollRecordsQuery, PagedResult<PayrollRecordResponseDto>>
{
    public async Task<PagedResult<PayrollRecordResponseDto>> Handle(GetAllPayrollRecordsQuery request, CancellationToken cancellationToken)
    {
        var pagedPayrollRecords = await repository.GetAllAsync(request.PageNumber, request.PageSize);
        return new PagedResult<PayrollRecordResponseDto>
        {
            Items = mapper.Map<IEnumerable<PayrollRecordResponseDto>>(pagedPayrollRecords.Items),
            TotalCount = pagedPayrollRecords.TotalCount,
            PageNumber = pagedPayrollRecords.PageNumber,
            PageSize = pagedPayrollRecords.PageSize
        };
    }
}

