using AutoMapper;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.AuditLog;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.AuditLogs.Queries.GetAllAuditLogs;

public class GetAllAuditLogsQueryHandler(IAuditLogRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAuditLogsQuery, PagedResult<AuditLogResponseDto>>
{
    public async Task<PagedResult<AuditLogResponseDto>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetAllAsync(request.PageNumber, request.PageSize, request.EntityName, request.EntityId);

        return new PagedResult<AuditLogResponseDto>
        {
            Items = mapper.Map<IEnumerable<AuditLogResponseDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
