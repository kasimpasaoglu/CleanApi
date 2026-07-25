namespace Web.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>> _exceptionHandlers = new()
    {
        { typeof(OperationCanceledException), HandleOperationCanceledException },
        { typeof(BusinessException), HandleBusinessException },
        { typeof(ValidationException), HandleValidationException },
        { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler.Invoke(httpContext, exception, ct);
            return true;
        }

        // Dev: UNHANDLED'I HİÇ HANDLE ETME
        if (httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            return false;

        // (şimdilik) Dev dışı: istersen fallback çalıştırırsın
        await HandleUnhandledException(httpContext, exception, ct);
        return true;
    }

    #region Business Exceptions

    private static async Task HandleBusinessException(HttpContext httpContext, Exception ex, CancellationToken ct)
    {
        // TODO: Loglama yapilacak
        var bx = (BusinessException)ex;

        // BusinessException içinde Error var (senin kurgu)
        var result = Result.Failure(bx.Error);

        var pd = ProblemDetailsFactory.FromResult(result);

        httpContext.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(pd, ct);
    }

    #endregion

    #region Validation Exceptions

    private static async Task HandleValidationException(HttpContext httpContext, Exception ex, CancellationToken ct)
    {
        var exception = (ValidationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        }, ct);
    }

    #endregion

    #region Authorization Exceptions

    private static async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex, CancellationToken ct)
    {
        //TODO: hatali giris denemeleri loglanacak
        var pd = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Yetkisiz erişim",
            Detail = ex.InnerException?.Message ?? ex.Message
        };
        await httpContext.Response.WriteAsJsonAsync(pd, ct);
    }

    #endregion

    #region OperationCanceledExceptions

    private static Task HandleOperationCanceledException(HttpContext httpContext, Exception ex, CancellationToken ct)
    {
        return Task.CompletedTask; // istek iptal edilirse hic bisey yapmadan sessizce geciyoruz
    }

    #endregion

    #region FallBack

    private static async Task HandleUnhandledException(HttpContext httpContext, Exception ex, CancellationToken ct)
    {
        // Burada Mongo log vb. yapacağın yer burası.

        if (httpContext.Response.HasStarted) // SignalR,Streaming response, file upload / download gibi durumlarda response başlamış olabilir, ikinci bir exception üretmemek için burada sadece loglama yapılır
        {
            // burada sadece loglanır, response'a dokunulmaz
            return;
        }

        ProblemDetails pd;

        try
        {
            pd = ProblemDetailsFactory.FromException(ex);
        }
        catch
        {
            pd = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Sunucu Hatasi",
                Detail = ex.Message
            };
        }

        httpContext.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(pd, ct);
    }

    #endregion
}