using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Country;

internal record GetAllCountriesRequest();

internal class GetAllCountriesHandler(ICountryDataReader countryDataReader)
    : IHandler<GetAllCountriesRequest, Result<IReadOnlyList<CountryDto>>>
{
    public async Task<Result<IReadOnlyList<CountryDto>>> HandleAsync(GetAllCountriesRequest request, CancellationToken cancellationToken)
    {
        var result = await countryDataReader.GetAllAsync(cancellationToken);

        if (!result.Success)
        {
            return Result<IReadOnlyList<CountryDto>>.Fail(result.ErrorMessage!);
        }

        var countries = result.Data!.Select(c => new CountryDto(c.Id, c.Name)).ToList();

        return Result<IReadOnlyList<CountryDto>>.Ok(countries);
    }
}
