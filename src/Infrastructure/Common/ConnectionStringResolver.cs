namespace Infrastructure.Common;

internal abstract class ConnectionStringResolver
{
    internal static string GetActiveConnectionString(IConfiguration config)
    {
        var active = config["Db:ActiveConnection"] ?? "DefaultConnection";
        var cs = config.GetConnectionString(active);

        return string.IsNullOrWhiteSpace(cs) ? throw new InvalidOperationException($"ConnectionStrings:{active} is missing.") : cs;
    }
}