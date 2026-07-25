namespace Domain.Entities.SchemaName;

public class SampleEntity : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Number { get; set; }
    public EnumSample Type { get; set; } = EnumSample.Value1;
}