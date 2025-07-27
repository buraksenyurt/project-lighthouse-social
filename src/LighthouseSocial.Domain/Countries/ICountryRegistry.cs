namespace LighthouseSocial.Domain.Countries;

public interface ICountryRegistry
{
    Task<Country> GetByIdAsync(int id);
    Task<IReadOnlyList<Country>> GetAllAsync();
}
