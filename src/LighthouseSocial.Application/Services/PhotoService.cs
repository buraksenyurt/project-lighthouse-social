using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class PhotoService(PipelineDispatcher pipelineDispatcher)
    : IPhotoService
{
    public async Task<bool> DeleteAsync(Guid photoId)
    {
        var request = new DeletePhotoRequest(photoId);
        var result = await pipelineDispatcher.SendAsync<DeletePhotoRequest, Result>(request);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to delete photo: {result.ErrorMessage}");
        }
        return result.Success;
    }

    public async Task<Guid> UploadAsync(PhotoDto dto, Stream fileContent)
    {
        var request = new UploadPhotoRequest(dto, fileContent);
        var result = await pipelineDispatcher.SendAsync<UploadPhotoRequest, Result<Guid>>(request);

        return result.Success
            ? result.Data
            : throw new InvalidOperationException($"Failed to upload photo: {result.ErrorMessage}");
    }
}
