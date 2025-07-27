using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

//todo@buraksenyurt Bu ve diğer Handler sınıfları Application dışına kapatılacak şekilde düzenlenmeli.
public class CreateLighthouseHandler(ILighthouseRepository repository, ICountryRegistry countryRegistry, IValidator<LighthouseDto> validator)
{
    private readonly ILighthouseRepository _repository = repository;
    private readonly ICountryRegistry _countryRegistry = countryRegistry;
    private readonly IValidator<LighthouseDto> _validator = validator;
    public async Task<Result<Guid>> HandleAsync(LighthouseDto dto)
    {
        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        Country? country;
        try
        {
            //todo@buraksenyurt Çok sık değişmeyecek Country bilgileri önbelleğe alınabilir.(Redis, MemoryCache vb.)
            country = await  _countryRegistry.GetByIdAsync(dto.CountryId);
        }
        catch (Exception ex)
        {
            //todo@buraksenyurt: Eğer mümkünse exception'ları loglamalıyız.
            return Result<Guid>.Fail($"Invalid country Id: {dto.CountryId}, {ex.Message}");
        }

        var location = new Coordinates(dto.Latitude, dto.Longitude);
        var lighthouse = new Domain.Entities.Lighthouse(dto.Name, country, location);

        await _repository.AddAsync(lighthouse);

        return Result<Guid>.Ok(lighthouse.Id);
    }
}
