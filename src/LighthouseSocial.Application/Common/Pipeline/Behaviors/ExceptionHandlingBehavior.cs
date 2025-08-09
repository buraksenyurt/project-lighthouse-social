using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    public ExceptionHandlingBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var requestType = request?.GetType().Name ?? "Unknown";

            _logger.LogError(ex, "Unhanled exception occured. {RequestName} of type {RequestType}", requestName, requestType);

            throw;
        }
    }
}
