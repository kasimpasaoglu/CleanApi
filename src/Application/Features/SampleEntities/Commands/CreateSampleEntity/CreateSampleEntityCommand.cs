namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

[Authorize(Policy = Policies.CanCreateSampleEntity)]
public record CreateSampleEntityCommand(string Name, int Number, EnumSample Type, string? Description) : IRequest<Result<CreateSampleEntityResponse>>;