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
        var lighthouse = await _repository.GetByIdAsync(request.LighthouseId);
        if (lighthouse == null)
        {
            return Result.Fail(Messages.Errors.Lighthouse.LighthouseNotFound);
        }

        var result = await _repository.DeleteAsync(request.LighthouseId);
        if (!result)
        {
            return Result.Fail(Messages.Errors.Lighthouse.FailedToDeleteLighthouse);
        }
        return Result.Ok();
    }
}
