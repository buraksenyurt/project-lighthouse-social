using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class PhotoService
    : IPhotoService
{
    private readonly PipelineDispatcher _pipelineDispatcher;
    public PhotoService(PipelineDispatcher pipelineDispatcher)
    {
        _pipelineDispatcher = pipelineDispatcher;
    }

    public async Task DeleteAsync(Guid photoId)
    {
        var request = new DeletePhotoRequest(photoId);
        //todo@buraksenyurt Geriye anlamlı response döndürmek daha iyi olur
        var _ = await _pipelineDispatcher.SendAsync<DeletePhotoRequest, Result>(request);
    }

    public async Task<Guid> UploadAsync(PhotoDto dto, Stream fileContent)
    {
        var request = new UploadPhotoRequest(dto, fileContent);
        var result = await _pipelineDispatcher.SendAsync<UploadPhotoRequest, Result<Guid>>(request);

        return result.Success
            ? result.Data
            : throw new InvalidOperationException($"Failed to upload photo: {result.ErrorMessage}");
    }
}
