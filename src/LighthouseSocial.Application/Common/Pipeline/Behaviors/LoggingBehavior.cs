using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Request handling was cancelled.");
            return default!;
        }
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Processing request {RequestName}", requestName);

        var response = await next();

        logger.LogInformation("Request {RequestName} completed", requestName);

        return response;
    }
}
