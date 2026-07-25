using Application.Events.SampleEntityCreated;

namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

public class CreateSampleEntityHandler(
    ISampleEntityRepository sampleEntityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateSampleEntityCommand, Result<CreateSampleEntityResponse>>
{
    public async Task<Result<CreateSampleEntityResponse>> Handle(CreateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var exists = await sampleEntityRepository.AnyAsync(
            x => x.Name == request.Name && x.Number == request.Number,
            cancellationToken);

        if (exists)
            return Result.Failure<CreateSampleEntityResponse>(
                Error.Conflict(ErrorCodes.AlreadyExists.SampleEntity,
                    $"'{request.Name}' adlı ve {request.Number} numaralı bir örnek kayıt zaten mevcut."));

        var entity = mapper.Map<SampleEntity>(request);

        await sampleEntityRepository.AddAsync(entity, cancellationToken);
        entity.AddDomainEvent(new SampleEntityCreatedDomainEvent(entity.Id, entity.Name));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mapper.Map<CreateSampleEntityResponse>(entity));
    }
}
