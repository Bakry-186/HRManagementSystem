using System.Security.Claims;
using System.Text.Json;
using HRM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HRM.Infrastructure.Interceptors;

public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await CaptureAuditLogsAsync(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task CaptureAuditLogsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var changedBy = httpContextAccessor.HttpContext?
            .User?.FindFirstValue(ClaimTypes.Name) ?? "System";

        var auditEntries = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => new AuditLog
            {
                Id         = Guid.NewGuid(),
                EntityName = e.Entity.GetType().Name,
                EntityId   = e.Entity.Id,
                Action     = e.State == EntityState.Added ? "Created" : "Updated",
                ChangedBy  = changedBy,
                ChangedAt  = DateTime.UtcNow,
                OldValues  = e.State == EntityState.Modified
                    ? Serialize(e.OriginalValues.Properties
                        .ToDictionary(p => p.Name, p => e.OriginalValues[p]?.ToString()))
                    : null,
                NewValues  = Serialize(e.CurrentValues.Properties
                    .ToDictionary(p => p.Name, p => e.CurrentValues[p]?.ToString()))
            })
            .ToList();

        if (auditEntries.Count > 0)
            await context.Set<AuditLog>().AddRangeAsync(auditEntries, cancellationToken);
    }

    private static string Serialize(Dictionary<string, string?> values) =>
        JsonSerializer.Serialize(values);
}
