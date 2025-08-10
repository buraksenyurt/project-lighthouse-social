using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class LighthouseService(PipelineDispatcher pipelineDispatcher)
    : ILighthouseService
{
    private readonly PipelineDispatcher _pipelineDispatcher = pipelineDispatcher;

    public async Task<Guid> CreateAsync(LighthouseDto dto)
    {
        var result = await _pipelineDispatcher.SendAsync<CreateLighthouseRequest, Result<Guid>>(new CreateLighthouseRequest(dto));
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create lighthouse: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public async Task<bool> DeleteAsync(Guid lighthouseId)
    {
        var result = await _pipelineDispatcher.SendAsync<DeleteLighthouseRequest, Result>(new DeleteLighthouseRequest(lighthouseId));
        if (!result.Success)
        {
            return false;
            //throw new InvalidOperationException($"Failed to delete lighthouse: {result.ErrorMessage}");
        }
        return result.Success;
    }

    public async Task<IEnumerable<LighthouseDto>> GetAllAsync()
    {
        var result = await _pipelineDispatcher.SendAsync<GetAllLighthouseRequest, Result<IEnumerable<LighthouseDto>>>(new GetAllLighthouseRequest());
        return result.Success ? result.Data : [];
    }

    public async Task<LighthouseDto?> GetByIdAsync(Guid lighthouseId)
    {
        var result = await _pipelineDispatcher.SendAsync<GetLighthouseByIdRequest, Result<LighthouseDto>>(new GetLighthouseByIdRequest(lighthouseId));
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to get lighthouse by id: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public async Task<IEnumerable<PhotoDto>> GetPhotosByIdAsync(Guid lighthouseId)
    {
        var result = await _pipelineDispatcher.SendAsync<GetPhotosByLighthouseRequest, Result<IEnumerable<PhotoDto>>>(new GetPhotosByLighthouseRequest(lighthouseId));
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to get photos for lighthouse: {result.ErrorMessage}");
        }
        return result.Data ?? [];
    }

    public async Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(Guid lighthouseId, LighthouseDto dto)
    {
        throw new NotImplementedException();
    }
}
