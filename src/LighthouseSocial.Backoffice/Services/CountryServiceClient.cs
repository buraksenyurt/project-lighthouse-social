using LighthouseSocial.Backoffice.Models;
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
    public CountryServiceClient(IHttpClientFactory httpClientFactory, ILogger<CountryServiceClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("LighthouseServiceClient");
        _logger = logger;
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
            var response = await _httpClient.GetAsync("country");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var countries = JsonSerializer.Deserialize<IEnumerable<CountryDto>>(content, _jsonSerializerOptions);
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
