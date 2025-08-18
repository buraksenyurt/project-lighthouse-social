using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Application.Features.Photo;

namespace LighthouseSocial.Application.Services;

public class LighthouseService(PipelineDispatcher pipelineDispatcher)
    : ILighthouseService
{
    private readonly PipelineDispatcher _pipelineDispatcher = pipelineDispatcher;

    public async Task<Result<Guid>> CreateAsync(LighthouseDto dto)
    {
        return await _pipelineDispatcher.SendAsync<CreateLighthouseRequest, Result<Guid>>(new CreateLighthouseRequest(dto));
    }

    public async Task<Result> DeleteAsync(Guid lighthouseId)
    {
        return await _pipelineDispatcher.SendAsync<DeleteLighthouseRequest, Result>(new DeleteLighthouseRequest(lighthouseId));
    }

    public async Task<Result<LighthouseDto>> GetByIdAsync(Guid lighthouseId)
    {
        return await _pipelineDispatcher.SendAsync<GetLighthouseByIdRequest, Result<LighthouseDto>>(new GetLighthouseByIdRequest(lighthouseId));
    }

    public async Task<Result<PagedResult<LighthouseDto>>> GetPagedAsync(PagingDto pagingDto)
    {
        return await _pipelineDispatcher.SendAsync<GetPagedLighthouseRequest,Result<PagedResult<LighthouseDto>>>(new GetPagedLighthouseRequest(pagingDto));
    }

    public async Task<Result<IEnumerable<PhotoDto>>> GetPhotosByIdAsync(Guid lighthouseId)
    {
        return await _pipelineDispatcher.SendAsync<GetPhotosByLighthouseRequest, Result<IEnumerable<PhotoDto>>>(new GetPhotosByLighthouseRequest(lighthouseId));
    }

    public async Task<Result<IEnumerable<LighthouseTopDto>>> GetTopAsync(TopDto topDto)
    {
        return await _pipelineDispatcher.SendAsync<GetTopLighthousesRequest, Result<IEnumerable<LighthouseTopDto>>>(new GetTopLighthousesRequest(topDto.Count));
    }

    public async Task<Result> UpdateAsync(Guid lighthouseId, LighthouseDto dto)
    {
        return await _pipelineDispatcher.SendAsync<UpdateLighthouseRequest, Result>(new UpdateLighthouseRequest(lighthouseId, dto));
    }
}
