using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Queries.GetAllAttendanceRecords;

public class GetAllAttendanceRecordsQueryHandler(IAttendanceRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAttendanceRecordsQuery, PagedResult<AttendanceRecordResponseDto>>
{
    public async Task<PagedResult<AttendanceRecordResponseDto>> Handle(
        GetAllAttendanceRecordsQuery request, CancellationToken cancellationToken)
    {
        var pagedAttendanceRecords = await repository.GetAllAsync(request.PageNumber, request.PageSize);

        return new PagedResult<AttendanceRecordResponseDto>
        {
            Items = mapper.Map<IEnumerable<AttendanceRecordResponseDto>>(pagedAttendanceRecords.Items),
            TotalCount = pagedAttendanceRecords.TotalCount,
            PageNumber = pagedAttendanceRecords.PageNumber,
            PageSize = pagedAttendanceRecords.PageSize
        };
    }
}
