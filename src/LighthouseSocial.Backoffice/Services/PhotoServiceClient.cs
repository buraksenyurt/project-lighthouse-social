using LighthouseSocial.Backoffice.Models;

namespace LighthouseSocial.Backoffice.Services;

public interface IPhotoServiceClient
{
    Task<ApiResponse<bool>> DeleteByIdAsync(Guid photoId);
}
public class PhotoServiceClient
    : IPhotoServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PhotoServiceClient> _logger;

    public PhotoServiceClient(IHttpClientFactory httpClientFactory, ILogger<PhotoServiceClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("LighthouseServiceClient");
        _logger = logger;
    }
    public async Task<ApiResponse<bool>> DeleteByIdAsync(Guid photoId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"photo/{photoId}");
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse<bool>
            {
                Success = false,
                ErrorMessage = $"API Error: {response.StatusCode} - {errorContent}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting photo");
            return new ApiResponse<bool>
            {
                Success = false,
                ErrorMessage = "Error deleting photo"
            };
        }
    }
}
