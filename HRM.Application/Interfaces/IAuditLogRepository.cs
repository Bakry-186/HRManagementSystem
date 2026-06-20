using HRM.Application.Common.Models;
using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<PagedResult<AuditLog>> GetAllAsync(int pageNumber, int pageSize, string? entityName = null, Guid? entityId = null);
}
