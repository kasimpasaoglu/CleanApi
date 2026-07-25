namespace Domain.Common;

public abstract class BaseSoftDeletableEntity<TId> : BaseAuditableEntity<TId>
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
}