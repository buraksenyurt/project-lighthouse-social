using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record UpdateLighthouseRequest(Guid LighthouseId, LighthouseDto Dto);

internal class UpdateLighthouseHandler(ILighthouseRepository repository)
    : IHandler<UpdateLighthouseRequest, Result>
{
    public async Task<Result> HandleAsync(UpdateLighthouseRequest request, CancellationToken cancellationToken)
    {
        var existingLighthouse = await repository.GetByIdAsync(request.LighthouseId);
        if (existingLighthouse is null)
        {
            return Result.Fail("Lighthouse not found.");
        }

        var country = Country.Create(
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

        var result = await repository.UpdateAsync(updatedLighthouse);

        if (!result)
        {
            return Result.Fail("Failed to update lighthouse.");
        }

        return Result.Ok();
    }
}
