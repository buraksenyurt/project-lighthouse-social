using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface IPhotoService
{
    Task<Result<Guid>> UploadAsync(PhotoDto dto, Stream fileContent);
    Task<Result> DeleteAsync(Guid photoId);
    Task<Result<Stream>> GetRawPhotoAsync(string fileName);
}
