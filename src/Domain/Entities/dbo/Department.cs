

namespace Domain.Entities.dbo;

public class Department : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
}