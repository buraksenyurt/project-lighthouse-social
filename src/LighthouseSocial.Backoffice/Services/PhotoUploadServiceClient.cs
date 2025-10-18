using LighthouseSocial.Backoffice.Models;

namespace LighthouseSocial.Backoffice.Services;

public interface IPhotoUploadServiceClient
{
    Task<ApiResponse<Guid>> UploadPhotoAsync(PhotoUploadRequest request, Stream fileStream, string fileName);
}

public class PhotoUploadServiceClient(IHttpClientFactory httpClientFactory, ILogger<PhotoUploadServiceClient> logger)
    : IPhotoUploadServiceClient
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("LighthouseServiceClient");

    public async Task<ApiResponse<Guid>> UploadPhotoAsync(PhotoUploadRequest request, Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "file", fileName);

            content.Add(new StringContent(request.FileName), "fileName");
            content.Add(new StringContent(request.CameraType), "cameraType");
            content.Add(new StringContent(request.UserId.ToString()), "userId");
            content.Add(new StringContent(request.LighthouseId.ToString()), "lighthouseId");
            content.Add(new StringContent(request.Resolution), "resolution");
            content.Add(new StringContent(request.Lens), "lens");
            content.Add(new StringContent(request.IsPrimary.ToString()), "isPrimary");

            var response = await httpClient.PostAsync("PhotoUpload/upload", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (Guid.TryParse(responseContent.Trim('"'), out var photoId))
                {
                    return new ApiResponse<Guid>
                    {
                        Success = true,
                        Data = photoId
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse<Guid>
            {
                Success = false,
                ErrorMessage = $"Failed to upload photo: {errorContent}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading photo for LighthouseId {LighthouseId}", request.LighthouseId);
            return new ApiResponse<Guid>
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred while uploading the photo."
            };
        }
    }
}
