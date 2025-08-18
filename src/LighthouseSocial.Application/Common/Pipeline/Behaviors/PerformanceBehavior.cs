using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
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
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        //todo@buraksenyurt 1000 değeri konfigürasyon dosyasından veya Vault'tan alınabilir
        if (elapsedMs > 1000)
        {
            logger.LogWarning("Slow request detected : {RequestName}, total duration is {ElapsedMs}", requestName, elapsedMs);
        }

        return response;
    }
}
