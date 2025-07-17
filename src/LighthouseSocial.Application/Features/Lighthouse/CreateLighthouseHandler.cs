using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

public class CreateLighthouseHandler(ILighthouseRepository repository, ICountryRegistry countryRegistry)
{
    private readonly ILighthouseRepository _repository = repository;
    private readonly ICountryRegistry _countryRegistry = countryRegistry;

    public async Task<Result<Guid>> HandleAsync(LighthouseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<Guid>.Fail("Lighthouse name is required.");

        Country? country;
        try
        {
            country = _countryRegistry.GetById(dto.CountryId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail($"Invalid country Id: {dto.CountryId}, {ex.Message}");
        }

        var location = new Coordinates(dto.Latitude, dto.Longitude);
        var lighthouse = new Domain.Entities.Lighthouse(dto.Name, country, location);

        await _repository.AddAsync(lighthouse);

        return Result<Guid>.Ok(lighthouse.Id);
    }
}
