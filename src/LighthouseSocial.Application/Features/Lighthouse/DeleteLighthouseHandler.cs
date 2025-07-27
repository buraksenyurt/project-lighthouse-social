using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Lighthouse;

public class DeleteLighthouseHandler(ILighthouseRepository repository)
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result> HandleAsync(Guid lighthouseId)
    {
        var lighthouse = await _repository.GetByIdAsync(lighthouseId);
        if (lighthouse == null)
        {
            return Result.Fail("Lighthouse not found");
        }

        await _repository.DeleteAsync(lighthouseId);
        return Result.Ok();
    }
}
