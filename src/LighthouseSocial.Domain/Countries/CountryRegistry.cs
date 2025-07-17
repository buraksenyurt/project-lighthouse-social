namespace LighthouseSocial.Domain.Countries;

public class CountryRegistry(IEnumerable<Country> countries)
    : ICountryRegistry
{
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
