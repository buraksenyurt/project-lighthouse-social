using LighthouseSocial.Backoffice.Models;
using System.Text;
using System.Text.Json;

namespace LighthouseSocial.Backoffice.Services;


public interface ILigthouseServiceClient
{
    Task<ApiResponse<Guid>> CreateAsync(CreateLighthouseRequest request);
    Task<ApiResponse<IEnumerable<LighthouseDto>>> GetPagedAsync(int pageNumber, int pageSize);
}

public class LigthouseServiceClient
    : ILigthouseServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LigthouseServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public LigthouseServiceClient(IHttpClientFactory httpClientFactory, ILogger<LigthouseServiceClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("LighthouseServiceClient");
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<Guid>> CreateAsync(CreateLighthouseRequest request)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("lighthouse", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if(Guid.TryParse(responseContent.Trim('"'), out var lighthouseId))
                {
                    return new ApiResponse<Guid>
                    {
                        Success = true,
                        Data = lighthouseId
                    };
                }               
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse<Guid>
            {
                Success = false,
                ErrorMessage = $"API Error: {response.StatusCode} - {errorContent}"
            };
        }
        catch( Exception ex)
        {
            _logger.LogError(ex, "Error creating lighthouse");
            return new ApiResponse<Guid>
            {
                Success = false,
                ErrorMessage = "Connection error occured while creating lighthouse"
            };
        }
    }

    public Task<ApiResponse<IEnumerable<LighthouseDto>>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }
}
