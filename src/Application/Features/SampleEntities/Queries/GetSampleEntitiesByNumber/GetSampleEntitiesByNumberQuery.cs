namespace Application.Features.SampleEntities.Queries.GetSampleEntitiesByNumber;

public record GetSampleEntitiesByNumberQuery(int Number) : IRequest<Result<List<GetSampleEntitiesByNumberResponse>>>;