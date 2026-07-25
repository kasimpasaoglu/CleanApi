using Domain.Entities.dbo;

namespace Infrastructure.Services;

public class AuditLogWriter(AppDbContext dbContext) : IAuditLogWriter
{
    public async Task WriteAsync(string tableName, string action, string performedById, string performedByName, string performedByIp, string? newValues, string? oldValues, string? keyValues)
    {
        var audit = new AuditLog
        {
            TableName = tableName,
            Action = action,
            PerformedById = performedById,
            PerformedByName =  performedByName,
            PerformedByIp = performedByIp,
            PerformedAt = DateTimeOffset.UtcNow,
            NewValues = newValues,
            OldValues = oldValues,
            KeyValues = keyValues
        };

        dbContext.AuditLogs.Add(audit);
        await dbContext.SaveChangesAsync();
    }
}