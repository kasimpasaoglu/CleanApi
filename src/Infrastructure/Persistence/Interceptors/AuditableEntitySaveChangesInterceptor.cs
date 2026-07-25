namespace Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = entry.Entity.CreatedDate == default ? dateTimeProvider.UtcNow : entry.Entity.CreatedDate;
                    entry.Entity.CreatedBy ??= currentUserService.UserId;
                    entry.Entity.CreatedByDepartmentId ??= currentUserService.UserDepartmentId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedDate = dateTimeProvider.UtcNow;
                    entry.Entity.LastModifiedBy = currentUserService.UserId;
                    break;
            }
        }
    }
}