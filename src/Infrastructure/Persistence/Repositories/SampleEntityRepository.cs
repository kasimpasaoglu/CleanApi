using Domain.Interfaces.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class SampleEntityRepository(AppDbContext dbContext) : EfRepository<SampleEntity, Guid>(dbContext), ISampleEntityRepository
{
    public async Task<List<SampleEntity>> ListByNumberAsync(int number, CancellationToken cancellationToken = default)
    {
        return await DbContext.SampleEntities.Where(x => x.Number == number).ToListAsync(cancellationToken);
    }
}