namespace LighthouseSocial.Domain.Countries;

public interface ICountryRegistry
{
    Country GetById(int id);
    IReadOnlyList<Country> GetAll();
}
