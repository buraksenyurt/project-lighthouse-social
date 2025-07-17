using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts;

public interface IPhotoService
{
    Task<Guid> UploadAsync(PhotoDto dto, Stream fileContent);
    Task DeleteAsync(Guid photoId);
}
