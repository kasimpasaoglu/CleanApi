namespace Domain.Common;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>
{
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public Guid? CreatedByDepartmentId { get; set; }
}