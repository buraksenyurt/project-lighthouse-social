using LighthouseSocial.Domain.Countries;

namespace LighthouseSocial.Application.Contracts;

public interface ICountryDataReader
{
    Task<Country> GetByIdAsync(int id);
    Task<IReadOnlyList<Country>> GetAllAsync();
}
