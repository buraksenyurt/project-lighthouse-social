using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling request of type {RequestType}", typeof(TRequest).Name);
        if (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Request handling was cancelled.");
            return default!;
        }

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var requestType = request?.GetType().Name ?? "Unknown";

            logger.LogError(ex, "Unhanled exception occured. {RequestName} of type {RequestType}", requestName, requestType);

            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Fail("Unexpected error occured.");
            }
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failMethod = typeof(Result).GetMethod("Fail", [typeof(string)])?.MakeGenericMethod(resultType);
                if (failMethod != null)
                {
                    var failResult = failMethod.Invoke(null, ["Beklenmeyen bir hata oluştu."]);
                    if (failResult is TResponse typedResult)
                    {
                        return typedResult;
                    }
                }
            }
        }
        return default!;
    }
}
