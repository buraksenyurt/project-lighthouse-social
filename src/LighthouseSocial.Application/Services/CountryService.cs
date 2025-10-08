using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Country;

namespace LighthouseSocial.Application.Services;

public class CountryService(PipelineDispatcher dispatcher)
    : ICountryService
{
    public async Task<Result<IReadOnlyList<CountryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dispatcher.SendAsync<GetAllCountriesRequest, Result<IReadOnlyList<CountryDto>>>(new GetAllCountriesRequest(), cancellationToken);
    }
}
