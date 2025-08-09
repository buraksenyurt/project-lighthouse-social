using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Lighthouse;
internal record GetAllLighthouseRequest();

internal class GetAllLighthousesHandler(ILighthouseRepository repository)
    : IHandler<GetAllLighthouseRequest, Result<IEnumerable<LighthouseDto>>>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result<IEnumerable<LighthouseDto>>> HandleAsync(GetAllLighthouseRequest request, CancellationToken cancellationToken)
    {
        var lighthouses = await _repository.GetAllAsync();

        if (!lighthouses.Any())
        {
            return Result<IEnumerable<LighthouseDto>>.Fail("No lighthouses found.");
        }

        var dtos = lighthouses.Select(l => new LighthouseDto(
            l.Id,
            l.Name,
            l.CountryId,
            l.Location.Latitude,
            l.Location.Longitude
        ));

        return Result<IEnumerable<LighthouseDto>>.Ok(dtos);
    }
}
