namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

public class CreateSampleEntityResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Number { get; set; }
    public EnumSample Type { get; set; } = EnumSample.Value1;
}