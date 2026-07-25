#pragma warning disable CA1873

namespace Web.Middleware;

public class RequestAuditMiddleware(RequestDelegate next, ILogger<RequestAuditMiddleware> logger)
{

    private const string HealthPath = "/health";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        //Direkt olarak loglanmayacaklar
        if (MatchesAny(path, EarlyExitPrefixes))
        {
            await next(context);
            return;
        }

        await next(context);

        var statusCode = context.Response.StatusCode;
        var user = context.User.Identity?.Name ?? "anonymous";
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var method = context.Request.Method;

        //health kodu 500 ve üzeri ise loglanacak, diğer durumlarda loglanmayacak
        if (path.StartsWithSegments(HealthPath, StringComparison.OrdinalIgnoreCase))
        {
            if (statusCode >= 500)
            {
                logger.LogWarning(
                    "Health check failed | Status: {StatusCode} | IP: {IP}",
                    statusCode, ip);
            }
            return;
        }

        if (MatchesAny(path, DebugDowngradePrefixes))
        {
            if (statusCode >= 400)
            {
                logger.LogWarning(
                    "Request {Method} {Path} -> {StatusCode} | User: {User} | IP: {IP}",
                    method, path, statusCode, user, ip);
            }
            else
            {
                logger.LogDebug(
                    "Request {Method} {Path} -> {StatusCode} | User: {User} | IP: {IP}",
                    method, path, statusCode, user, ip);
            }
            return;
        }

        if (statusCode >= 400)
        {
            logger.LogWarning(
                "Request {Method} {Path} -> {StatusCode} | User: {User} | IP: {IP}",
                method, path, statusCode, user, ip);
        }
        else if (HttpMethods.IsGet(method))
        {
            logger.LogDebug(
                "Request {Method} {Path} -> {StatusCode} | User: {User} | IP: {IP}",
                method, path, statusCode, user, ip);
        }
        else
        {
            logger.LogInformation(
                "Request {Method} {Path} -> {StatusCode} | User: {User} | IP: {IP}",
                method, path, statusCode, user, ip);
        }
    }

    private static bool MatchesAny(PathString path, string[] prefixes)
    {
        foreach (var prefix in prefixes)
        {
            if (path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static readonly string[] EarlyExitPrefixes =
    {
        "/swagger",
        "/swagger-ui",
        "/favicon.ico",
        "/_framework",
        "/_content",
    };

    private static readonly string[] DebugDowngradePrefixes =
    {
    };

}
