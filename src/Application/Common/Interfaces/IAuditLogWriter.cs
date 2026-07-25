namespace Application.Common.Interfaces;

public interface IAuditLogWriter
{
    Task WriteAsync(string tableName, string action, string performedById, string perfomedByName, string perfomedByIp, string? newValues, string? oldValues, string? keyValues);
}