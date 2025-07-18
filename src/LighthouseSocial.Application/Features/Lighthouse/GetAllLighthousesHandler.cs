using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Lighthouse;

public class GetAllLighthousesHandler(ILighthouseRepository repository)
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result<IEnumerable<LighthouseDto>>> HandleAsync()
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
