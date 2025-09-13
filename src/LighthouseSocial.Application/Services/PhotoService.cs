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

    public async Task<Result<Stream>> GetRawPhotoAsync(string fileName)
    {
        var request = new GetRawPhotoRequest(fileName);
        return await pipelineDispatcher.SendAsync<GetRawPhotoRequest, Result<Stream>>(request);
    }

    public async Task<Result<PhotoDto>> GetByIdAsync(Guid photoId)
    {
        var request = new GetPhotoByIdRequest(photoId);
        return await pipelineDispatcher.SendAsync<GetPhotoByIdRequest, Result<PhotoDto>>(request);
    }

    public async Task<Result<IEnumerable<PhotoDto>>> GetByUserIdAsync(Guid userId)
    {
        var request = new GetPhotosByUserRequest(userId);
        return await pipelineDispatcher.SendAsync<GetPhotosByUserRequest, Result<IEnumerable<PhotoDto>>>(request);
    }
}
