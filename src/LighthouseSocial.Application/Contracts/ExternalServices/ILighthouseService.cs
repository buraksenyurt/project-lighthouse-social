using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface ILighthouseService
{
    Task<Result<IEnumerable<LighthouseTopDto>>> GetTopAsync(TopDto topDto);
    Task<Result<IEnumerable<PhotoDto>>> GetPhotosByIdAsync(Guid lighthouseId);
    Task<Result<LighthouseDto>> GetByIdAsync(Guid lighthouseId);
    Task<Result<Guid>> CreateAsync(LighthouseDto dto);
    Task<Result> UpdateAsync(Guid lighthouseId, LighthouseDto dto);
    Task<Result> DeleteAsync(Guid lighthouseId);
    Task<Result<PagedResult<LighthouseDto>>> GetPagedAsync(PagingDto pagingDto);
}
