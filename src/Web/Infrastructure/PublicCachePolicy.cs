namespace Web.Infrastructure;

public class PublicCachePolicy(TimeSpan ttl) : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var req = context.HttpContext.Request;

        // Sadece GET/HEAD cache
        var cacheableMethod = HttpMethods.IsGet(req.Method) || HttpMethods.IsHead(req.Method);
        if (!cacheableMethod)
            return ValueTask.CompletedTask;

        // DefaultPolicy'nin "Authorization varsa bypass" davranışını burada bilerek override ediyoruz.
        // Bu policy yalnızca user-agnostic (shared) endpoint'lere uygulanır; user-specific data için kullanılmamalıdır.
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        context.ResponseExpirationTimeSpan = ttl;

        // Query parametreleri cache key'e dahil olsun (cityId gibi)
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var res = context.HttpContext.Response;

        // Set-Cookie varsa cache'leme (güvenlik)
        if (!StringValues.IsNullOrEmpty(res.Headers.SetCookie))
            context.AllowCacheStorage = false;

        // 200/301 dışı cache'leme
        if (res.StatusCode is not (StatusCodes.Status200OK or StatusCodes.Status301MovedPermanently))
            context.AllowCacheStorage = false;

        return ValueTask.CompletedTask;
    }
}