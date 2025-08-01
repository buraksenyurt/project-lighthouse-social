using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class PhotoService(UploadPhotoHandler uploadPhotoHandler)
            : IPhotoService
{
    private readonly UploadPhotoHandler _uploadPhotoHandler = uploadPhotoHandler;

    public async Task DeleteAsync(Guid photoId)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> UploadAsync(PhotoDto dto, Stream fileContent)
    {
        var result = await _uploadPhotoHandler.HandleAsync(dto, fileContent);
        return result.Success
            ? result.Data
            : throw new InvalidOperationException($"Failed to upload photo: {result.ErrorMessage}");
    }
}
