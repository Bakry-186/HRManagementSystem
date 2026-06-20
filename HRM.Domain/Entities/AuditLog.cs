namespace HRM.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public required string EntityName { get; set; }
    public required Guid EntityId { get; set; }
    public required string Action { get; set; }
    public required string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? OldValues { get; set; }
    public required string NewValues { get; set; }
}
