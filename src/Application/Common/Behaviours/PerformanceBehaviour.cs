#pragma warning disable CA1873
namespace Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        if (_timer.ElapsedMilliseconds > 1000)
        {
            logger.LogWarning("Long Running Request: {Name} ({Elapsed} ms) ",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds);
        }

        return response;
    }
}