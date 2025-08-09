using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;

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

    public async Task DeleteAsync(Guid id)
    {
        var result = await _pipelineDispatcher.SendAsync<DeleteLighthouseRequest, Result>(new DeleteLighthouseRequest(id));
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create lighthouse: {result.ErrorMessage}");
        }
    }

    public async Task<IEnumerable<LighthouseDto>> GetAllAsync()
    {
        var result = await _pipelineDispatcher.SendAsync<GetAllLighthouseRequest, Result<IEnumerable<LighthouseDto>>>(new GetAllLighthouseRequest());
        return result.Success ? result.Data : [];
    }

    public async Task<LighthouseDto?> GetByIdAsync(Guid id)
    {
        var result = await _pipelineDispatcher.SendAsync<GetLighthouseByIdRequest, Result<LighthouseDto>>(new GetLighthouseByIdRequest(id));
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to get lighthouse by id: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public async Task<IEnumerable<Photo>> GetPhotosByIdAsync(Guid photoId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Guid id, LighthouseDto dto)
    {
        throw new NotImplementedException();
    }
}
