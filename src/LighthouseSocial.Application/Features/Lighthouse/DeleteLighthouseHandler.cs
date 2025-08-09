using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Features.Models;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal class DeleteLighthouseHandler(ILighthouseRepository repository)
    : IHandler<DeleteLighthouseRequest, Result>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result> HandleAsync(DeleteLighthouseRequest request, CancellationToken cancellationToken)
    {
        var lighthouse = await _repository.GetByIdAsync(request.LighthouseId);
        if (lighthouse == null)
        {
            return Result.Fail("Lighthouse not found");
        }

        await _repository.DeleteAsync(request.LighthouseId);
        return Result.Ok();
    }
}
