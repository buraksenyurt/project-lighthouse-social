using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts;

public interface IPhotoService
{
    Task<Guid> UploadAsync(PhotoDto dto, Stream fileContent);
    Task<bool> DeleteAsync(Guid photoId);
}
