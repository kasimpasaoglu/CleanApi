namespace Application.Features.SampleEntities.Queries.GetSampleEntitiesByNumber;

public class GetSampleEntitiesByNumberResponse
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Number { get; set; }
    public EnumSample Type { get; set; } = EnumSample.Value1;
}