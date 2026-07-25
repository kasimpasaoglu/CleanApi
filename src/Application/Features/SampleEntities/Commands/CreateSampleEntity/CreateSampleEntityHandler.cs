namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

public class CreateSampleEntityHandler(
    ISampleEntityRepository sampleEntityRepository,
    IMapper mapper) : IRequestHandler<CreateSampleEntityCommand, Result<CreateSampleEntityResponse>>
{
    public async Task<Result<CreateSampleEntityResponse>> Handle(CreateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        await sampleEntityRepository.AddAsync(mapper.Map<SampleEntity>(request), cancellationToken);
        var response = sampleEntityRepository.Query(asNoTracking: true)
            .Where(x => x.Name == request.Name
                        && x.Number == request.Number
                        && x.Type == request.Type
                        && x.Description == request.Description)
            .FirstOrDefaultAsync(cancellationToken);
        return Result.Success(mapper.Map<CreateSampleEntityResponse>(response));
    }
}