namespace LighthouseSocial.Domain.Countries;

public class CountryRegistry(IEnumerable<Country> countries)
    : ICountryRegistry
{
    // Bunun yeri doğru mu? Data'yı Referans edemeyiz zira Data, Domain'i referans ediyor.
    // Circular Dependency sorunu oluştur.
    
    //todo@buraksenyurt Veritabanından beslenmesi gerekiyor?
    private readonly Dictionary<int, Country> _countries = countries.ToDictionary(c => c.Id);

    public IReadOnlyList<Country> GetAll()
    {
        return [.. _countries.Values];
    }

    public Country GetById(int id)
    {
        return _countries.TryGetValue(id, out var country)
            ? country 
            : throw new KeyNotFoundException($"Country id not found:{id}");
    }
}
