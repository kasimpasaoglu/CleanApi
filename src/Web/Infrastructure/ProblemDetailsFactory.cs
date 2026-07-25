namespace Web.Infrastructure;

public static class ProblemDetailsFactory
{
    public static ProblemDetails FromResult(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot create ProblemDetails from a successful Result.");

        var status = MapStatus(result.Error.Type);
        var type = MapType(result.Error.Type);

        var pd = new ProblemDetails
        {
            Status = status,
            Type = type,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        // Validation errors (extensions)
        if (result.Error is ValidationError ve)
            pd.Extensions["errors"] = ve.Errors;

        // traceId vs gibi ek şeyler burada verilmez; çünkü HttpContext yok.
        return pd;
    }
    
    public static ProblemDetails FromException(Exception ex)
    {
        // geri kalan exceptionlarda detail kismini opsiyonel yapmasin, Innerexception varsa onu yazsin yoksa ex.message'i bassin her zaman dolu gelsin 
        // Unhandled fallback
        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError, // status kodu sabit 500 yerine MapSatatus ile almak daha dogru olur?
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Sunucu hatası",
            Detail = ResolveDetail(ex),
        };
    }
    
    
    
    private static string ResolveDetail(Exception ex)
    {
        // InnerException zincirinde en anlamlı mesaj
        var current = ex;
        while (current.InnerException != null)
            current = current.InnerException;

        return current.Message;
    }
    
    private static int MapStatus(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string MapType(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };
}