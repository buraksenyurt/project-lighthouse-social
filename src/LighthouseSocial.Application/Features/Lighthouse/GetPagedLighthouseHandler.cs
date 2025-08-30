using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record GetPagedLighthouseRequest(PagingDto PagingDto);

internal class GetPagedLighthouseHandler(ILighthouseRepository lighthouseRepository)
    : IHandler<GetPagedLighthouseRequest, Result<PagedResult<LighthouseDto>>>
{
    public async Task<Result<PagedResult<LighthouseDto>>> HandleAsync(GetPagedLighthouseRequest request, CancellationToken cancellationToken)
    {
        var pagedResult = await lighthouseRepository.GetPagedAsync(request.PagingDto.Skip, request.PagingDto.PageSize);
        
        if (!pagedResult.Success)
        {
            return Result<PagedResult<LighthouseDto>>.Fail(pagedResult.ErrorMessage!);
        }

        var (lighthouses, totalCount) = pagedResult.Data!;

        var dtos = lighthouses.Select(l => new LighthouseDto
        (
            l.Id,
            l.Name,
            l.CountryId,
            l.Location.Latitude,
            l.Location.Longitude
        )).ToList();

        var pagedResultDto = new PagedResult<LighthouseDto>
        (
            dtos,
           totalCount,
           request.PagingDto.Page,
            request.PagingDto.PageSize
        );

        return Result<PagedResult<LighthouseDto>>.Ok(pagedResultDto);
    }
}
