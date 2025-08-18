using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class PhotoService(PipelineDispatcher pipelineDispatcher)
    : IPhotoService
{
    public async Task<Result> DeleteAsync(Guid photoId)
    {
        var request = new DeletePhotoRequest(photoId);
        return await pipelineDispatcher.SendAsync<DeletePhotoRequest, Result>(request);
    }

    public async Task<Result<Guid>> UploadAsync(PhotoDto dto, Stream fileContent)
    {
        var request = new UploadPhotoRequest(dto, fileContent);
        return await pipelineDispatcher.SendAsync<UploadPhotoRequest, Result<Guid>>(request);
    }

    public async Task<Result<Stream>> GetRawPhotoAsync(string fileName)
    {
        var request = new GetRawPhotoRequest(fileName);
        return await pipelineDispatcher.SendAsync<GetRawPhotoRequest, Result<Stream>>(request);
    }
}
