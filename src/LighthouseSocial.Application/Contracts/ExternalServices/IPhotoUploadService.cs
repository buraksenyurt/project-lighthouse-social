using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface IPhotoUploadService
{
    Task<Result<PhotoDto>> UploadAsync(PhotoDto dto, Stream fileContent);
}
