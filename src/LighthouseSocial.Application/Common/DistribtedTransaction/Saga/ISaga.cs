namespace LighthouseSocial.Application.Common.DistribtedTransaction.Saga;

public interface ISaga<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface ISagaStep<TStepData>
{
    Task<Result<TStepData>> ExecuteAsync(TStepData data, CancellationToken cancellationToken = default);
    Task CompensateAsync(TStepData data, CancellationToken cancellationToken = default);
}
