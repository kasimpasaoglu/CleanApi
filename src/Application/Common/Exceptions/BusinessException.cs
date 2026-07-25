namespace Application.Common.Exceptions;

public sealed class BusinessException(Error error) : Exception(error.Description)
{
    public Error Error { get; } = error;
}

public static class BusinessExceptionResultExtensions
{
    public static Result<T> ToFailureResult<T>(this BusinessException ex)
        => Result.Failure<T>(ex.Error);
}