using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ICountryDataReader
{
    Task<Result<Country>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Country>>> GetAllAsync(CancellationToken cancellationToken = default);
}
