using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface ILighthouseService
{
    Task<Result<IEnumerable<LighthouseTopDto>>> GetTopAsync(TopDto topDto);
    Task<Result<IEnumerable<PhotoDto>>> GetPhotosByIdAsync(Guid lighthouseId);
    Task<Result<LighthouseDto>> GetByIdAsync(Guid lighthouseId);
    Task<Result<Guid>> CreateAsync(LighthouseUpsertDto dto);
    Task<Result> UpdateAsync(Guid lighthouseId, LighthouseUpsertDto dto);
    Task<Result> DeleteAsync(Guid lighthouseId);
    Task<Result<PagedResult<LighthouseDto>>> GetPagedAsync(PagingDto pagingDto);
}
