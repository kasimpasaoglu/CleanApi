namespace Application.Features.SampleEntities.Queries.GetSampleEntitiesByNumber;

public class GetSampleEntitiesByNumberHandler(
    ISampleEntityRepository sampleEntityRepository,
    IMapper mapper) : IRequestHandler<GetSampleEntitiesByNumberQuery, Result<List<GetSampleEntitiesByNumberResponse>>>
{
    public async Task<Result<List<GetSampleEntitiesByNumberResponse>>> Handle(GetSampleEntitiesByNumberQuery request, CancellationToken cancellationToken)
    {
        var list = await sampleEntityRepository.ListByNumberAsync(request.Number, cancellationToken);
        var response = mapper.Map<List<GetSampleEntitiesByNumberResponse>>(list);

        return Result.Success(response);
    }
}