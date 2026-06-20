using HRM.Application.Common.Models;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class AuditLogRepository(AppDbContext context) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog log)
    {
        await context.AuditLogs.AddAsync(log);
    }

    public async Task<PagedResult<AuditLog>> GetAllAsync(int pageNumber, int pageSize, string? entityName = null, Guid? entityId = null)
    {
        var query = context.AuditLogs.AsQueryable();
        if (!string.IsNullOrEmpty(entityName))
        {
            query = query.Where(e => e.EntityName == entityName);
        }
        if (entityId.HasValue)
        {
            query = query.Where(e => e.EntityId == entityId.Value);
        }

        var totalCount = await query.CountAsync();
        var pagedLogs = await query.OrderByDescending(e => e.ChangedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedResult<AuditLog>
        {
            Items = pagedLogs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
