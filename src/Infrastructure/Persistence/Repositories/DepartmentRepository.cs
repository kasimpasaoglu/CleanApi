namespace Infrastructure.Persistence.Repositories;

public class DepartmentRepository(AppDbContext dbContext) : EfRepository<Department, Guid>(dbContext), IDepartmentRepository
{
    
}