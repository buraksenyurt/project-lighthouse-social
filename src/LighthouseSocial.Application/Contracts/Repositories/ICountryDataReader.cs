using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ICountryDataReader
{
    Task<Country> GetByIdAsync(int id);
    Task<IReadOnlyList<Country>> GetAllAsync();
}
