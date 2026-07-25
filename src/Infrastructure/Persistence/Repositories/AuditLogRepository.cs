
namespace Infrastructure.Persistence.Repositories;

public class AuditLogRepository(AppDbContext dbContext) : EfRepository<AuditLog, Guid>(dbContext), IAuditLogRepository
{
    
}