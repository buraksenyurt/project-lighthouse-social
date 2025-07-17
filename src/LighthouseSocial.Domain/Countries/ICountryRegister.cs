namespace LighthouseSocial.Domain.Countries;

public interface ICountryRegister
{
    Country GetById(int id);
    IReadOnlyList<Country> GetAll();
}
