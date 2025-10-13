using LighthouseSocial.Backoffice.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace LighthouseSocial.Backoffice.Services;

public interface ICountryServiceClient
{
    Task<ApiResponse<IEnumerable<CountryDto>>> GetAllAsync();
}
public class CountryServiceClient
    : ICountryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CountryServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMemoryCache _memoryCache;
    private const string CountriesCacheKey = "countries_key";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(3);
    public CountryServiceClient(IHttpClientFactory httpClientFactory, ILogger<CountryServiceClient> logger, IMemoryCache memoryCache)
    {
        _httpClient = httpClientFactory.CreateClient("LighthouseServiceClient");
        _logger = logger;
        _memoryCache = memoryCache;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
    public async Task<ApiResponse<IEnumerable<CountryDto>>> GetAllAsync()
    {
        try
        {
            if (_memoryCache.TryGetValue(CountriesCacheKey, out IEnumerable<CountryDto>? cachedCountries))
            {
                _logger.LogInformation("Countries retrieved from cache.");
                return new ApiResponse<IEnumerable<CountryDto>>
                {
                    Success = true,
                    Data = cachedCountries ?? []
                };
            }


            _logger.LogInformation("Fetching countries from CountryService API.");
            var response = await _httpClient.GetAsync("country");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var countries = JsonSerializer.Deserialize<IEnumerable<CountryDto>>(content, _jsonSerializerOptions);

                if (countries != null)
                {
                    _memoryCache.Set(CountriesCacheKey, countries, CacheDuration);
                    _logger.LogInformation("Countries cached for {CacheDuration} hours.", CacheDuration.TotalHours);
                }

                return new ApiResponse<IEnumerable<CountryDto>>
                {
                    Success = true,
                    Data = countries ?? []
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse<IEnumerable<CountryDto>>
            {
                Success = false,
                ErrorMessage = $"API Error: {response.StatusCode} - {errorContent}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while fetching countries from CountryService");
            return new ApiResponse<IEnumerable<CountryDto>>
            {
                Success = false,
                ErrorMessage = "Connection error occurred while fetching countries"
            };
        }
    }
}
