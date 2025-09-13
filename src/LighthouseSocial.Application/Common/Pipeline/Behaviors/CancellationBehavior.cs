using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Common.Pipeline.Behaviors;

public class CancellationBehavior<TRequest, TResponse>(ILogger<CancellationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogWarning("Request {RequestName} was cancelled before processing", requestName);

            return HandleCancellation();
        }

        return await next();
    }

    private static TResponse HandleCancellation()
    {
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var failMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Fail", new[] { typeof(string) });
            return (TResponse)failMethod!.Invoke(null, new object[] { "Operation was cancelled" })!;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Fail("Operation was cancelled");
        }

        throw new OperationCanceledException("Operation was cancelled");
    }
}
