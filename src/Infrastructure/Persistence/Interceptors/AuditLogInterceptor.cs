namespace Infrastructure.Persistence.Interceptors;

public class AuditLogInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return result;

        var auditEntries = CreateAuditEntries(eventData.Context);
        if (auditEntries.Count == 0) return result;
        var auditSet = eventData.Context.Set<AuditLog>();
        await auditSet.AddRangeAsync(auditEntries, cancellationToken);

        return result;
    }

    private List<AuditLog> CreateAuditEntries(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted &&
                        e.Entity is not AuditLog);

        var audits = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var audit = new AuditLog
            {
                TableName = entry.Metadata.GetTableName() ?? "",
                Action = entry.State.ToString(),
                PerformedAt = DateTimeOffset.UtcNow,
                PerformedById = currentUserService.UserId ?? "system",
                PerformedByName = currentUserService.FullName ?? "system",
                PerformedByIp = currentUserService.IpAddress ?? "unknown",
                KeyValues = SerializeKeys(entry),
                OldValues = entry.State is EntityState.Modified or EntityState.Deleted
                    ? SerializeProperties(entry, original: true)
                    : null,
                NewValues = entry.State is EntityState.Modified or EntityState.Added
                    ? SerializeProperties(entry, original: false)
                    : null
            };

            audits.Add(audit);
        }

        return audits;
    }

    private static string SerializeKeys(EntityEntry entry) =>
        JsonSerializer.Serialize(entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));

    private static string SerializeProperties(EntityEntry entry, bool original) =>
        JsonSerializer.Serialize(entry.Properties
            .ToDictionary(p => p.Metadata.Name,
                p => original ? p.OriginalValue : p.CurrentValue));
}