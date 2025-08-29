using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ICountryDataReader
{
    Task<Result<Country>> GetByIdAsync(int id);
    Task<IReadOnlyList<Country>> GetAllAsync();
}
