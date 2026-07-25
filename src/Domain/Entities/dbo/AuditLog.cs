namespace Domain.Entities.dbo;

public class AuditLog : BaseEntity<Guid>
{
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? KeyValues { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? PerformedById { get; set; }
    public string? PerformedByName { get; set; }
    public string? PerformedByIp { get; set; }
    public DateTimeOffset PerformedAt { get; set; }
}