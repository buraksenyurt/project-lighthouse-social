using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface ICountryService
{
    Task<Result<IReadOnlyList<CountryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}
