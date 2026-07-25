namespace Domain.Interfaces.Repositories;

public interface ISampleEntityRepository : IRepository<SampleEntity, Guid>
{
    // Custom repository methods goes here
    Task<List<SampleEntity>> ListByNumberAsync(int number, CancellationToken cancellationToken);
}