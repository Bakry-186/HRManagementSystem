using HRM.Application.Common.Models;
using HRM.Application.DTOs.AuditLog;
using MediatR;

namespace HRM.Application.Features.AuditLogs.Queries.GetAllAuditLogs;

public record GetAllAuditLogsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? EntityName = null,
    Guid? EntityId = null) : IRequest<PagedResult<AuditLogResponseDto>>;
