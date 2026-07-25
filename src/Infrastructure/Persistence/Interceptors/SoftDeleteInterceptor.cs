using Domain.Interfaces;
using Infrastructure.Identity;

namespace Infrastructure.Persistence.Interceptors;

public class SoftDeleteInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        HandleSoftDelete(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleSoftDelete(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted &&
                        e.Entity is BaseSoftDeletableEntity<Guid>);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
            var now = DateTimeOffset.UtcNow;
            
            switch (entry.Entity)
            {
                case ApplicationUser u:
                    u.SoftDelete(userName, now);
                    break;
                
                case BaseSoftDeletableEntity<Guid> baseSoft:
                    baseSoft.IsDeleted = true;
                    baseSoft.DeletedDate = now;
                    baseSoft.DeletedBy = userName;
                    break;
            }
        }
    }
}