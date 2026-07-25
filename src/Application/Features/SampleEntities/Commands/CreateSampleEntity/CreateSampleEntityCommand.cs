namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

public record CreateSampleEntityCommand(string Name, int Number, EnumSample Type, string? Description) : IRequest<Result<CreateSampleEntityResponse>>;