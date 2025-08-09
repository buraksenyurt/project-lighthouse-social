using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Processing request {RequestName}", requestName);

        var response = await next();

        _logger.LogInformation("Request {RequestName} completed", requestName);

        return response;
    }
}
