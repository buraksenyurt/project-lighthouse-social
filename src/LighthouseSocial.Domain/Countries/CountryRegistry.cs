namespace LighthouseSocial.Domain.Countries;

[Obsolete("This class is obsolete and will be removed in a future version. Use the new CountryRepository instead.")]
public class CountryRegistry(IEnumerable<Country> countries)
    : ICountryRegistry
{
    private readonly Dictionary<int, Country> _countries = countries.ToDictionary(c => c.Id);

    public Task<IReadOnlyList<Country>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Country>>([.. _countries.Values]);
    }

    public Task<Country> GetByIdAsync(int id)
    {
        return Task.FromResult(_countries.TryGetValue(id, out var country)
             ? country
             : throw new KeyNotFoundException($"Country id not found:{id}"));
    }
}
