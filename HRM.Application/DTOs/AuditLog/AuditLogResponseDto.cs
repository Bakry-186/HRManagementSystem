namespace HRM.Application.DTOs.AuditLog;

public class AuditLogResponseDto
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? OldValues { get; set; }
    public string NewValues { get; set; } = string.Empty;
}
