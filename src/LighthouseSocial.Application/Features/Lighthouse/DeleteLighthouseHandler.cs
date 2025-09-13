using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record DeleteLighthouseRequest(Guid LighthouseId);

internal class DeleteLighthouseHandler(ILighthouseRepository repository)
    : IHandler<DeleteLighthouseRequest, Result>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result> HandleAsync(DeleteLighthouseRequest request, CancellationToken cancellationToken)
    {
        var lighthouseResult = await _repository.GetByIdAsync(request.LighthouseId, cancellationToken);
        if (!lighthouseResult.Success)
        {
            return Result.Fail(lighthouseResult.ErrorMessage!);
        }

        var deleteResult = await _repository.DeleteAsync(request.LighthouseId, cancellationToken);
        if (!deleteResult.Success)
        {
            return Result.Fail(deleteResult.ErrorMessage!);
        }

        return Result.Ok();
    }
}
