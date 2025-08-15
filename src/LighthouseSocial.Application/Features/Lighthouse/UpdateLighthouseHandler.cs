using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record UpdateLighthouseRequest(Guid LighthouseId, LighthouseDto Dto);

internal class UpdateLighthouseHandler
    : IHandler<UpdateLighthouseRequest, Result>
{
    private readonly ILighthouseRepository _repository;
    public async Task<Result> HandleAsync(UpdateLighthouseRequest request, CancellationToken cancellationToken)
    {
        var existingLighthouse = _repository.GetByIdAsync(request.LighthouseId);
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

        await _repository.UpdateAsync(updatedLighthouse);

        return Result.Ok();
    }
}
