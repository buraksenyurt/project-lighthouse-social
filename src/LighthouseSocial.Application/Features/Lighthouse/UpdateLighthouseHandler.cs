using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record UpdateLighthouseRequest(Guid LighthouseId, LighthouseUpsertDto Dto);

internal class UpdateLighthouseHandler(ILighthouseRepository repository)
    : IHandler<UpdateLighthouseRequest, Result>
{
    public async Task<Result> HandleAsync(UpdateLighthouseRequest request, CancellationToken cancellationToken)
    {
        var existingLighthouseResult = await repository.GetByIdAsync(request.LighthouseId, cancellationToken);
        if (!existingLighthouseResult.Success)
        {
            return Result.Fail(existingLighthouseResult.ErrorMessage!);
        }

        var country = Domain.Entities.Country.Create(
            request.Dto.CountryId, ""
        );
        var coordinates = new Coordinates(
            request.Dto.Latitude,
            request.Dto.Longitude
        );
        var updatedLighthouse = new Domain.Entities.Lighthouse(
            request.LighthouseId,
            request.Dto.Name,
            country,
            coordinates
        );

        var updateResult = await repository.UpdateAsync(updatedLighthouse, cancellationToken);
        if (!updateResult.Success)
        {
            return Result.Fail(updateResult.ErrorMessage!);
        }

        return Result.Ok();
    }
}
