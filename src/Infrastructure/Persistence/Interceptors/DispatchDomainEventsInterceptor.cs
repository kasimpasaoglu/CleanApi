namespace Infrastructure.Persistence.Interceptors;

public class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    // DbContext başına geçici bir event havuzu
    private readonly Dictionary<DbContext, List<BaseEvent>> _buffer = new();

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData e, InterceptionResult<int> result, CancellationToken ct)
    {
        var ctx = e.Context;
        if (ctx is null) return await base.SavingChangesAsync(e, result, ct);

        if (!_buffer.TryGetValue(ctx, out var list))
        {
            list = new List<BaseEvent>();
            _buffer[ctx] = list;
        }

        // eventleri topla ve entity’den temizle
        foreach (var entry in ctx.ChangeTracker.Entries<BaseEntity<Guid>>())
        {
            if (entry.Entity.DomainEvents.Count == 0) continue;
            list.AddRange(entry.Entity.DomainEvents);
            entry.Entity.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(e, result, ct);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData e, int result, CancellationToken ct)
    {
        var ctx = e.Context;
        if (ctx is null) return await base.SavedChangesAsync(e, result, ct);

        if (!_buffer.TryGetValue(ctx, out var events) || events.Count == 0)
            return await base.SavedChangesAsync(e, result, ct);

        // 🔑 kritik: önce snapshot al ve buffer’ı KALDIR
        var snapshot = events.ToArray();
        _buffer.Remove(ctx); // reentrancy’de ikinci SavedChanges boş görsün

        // sonra publish et
        foreach (var ev in snapshot)
            await publisher.Publish(ev, ct);

        return await base.SavedChangesAsync(e, result, ct);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData e, CancellationToken ct)
    {
        if (e.Context is not null) _buffer.Remove(e.Context);
        return base.SaveChangesFailedAsync(e, ct);
    }
}