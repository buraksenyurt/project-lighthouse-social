using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts;

public interface ILighthouseService
{
    Task<Result<IEnumerable<LighthouseDto>>> GetAllAsync();
    Task<Result<IEnumerable<LighthouseDto>>> GetTopAsync(TopDto topDto);
    Task<Result<IEnumerable<PhotoDto>>> GetPhotosByIdAsync(Guid lighthouseId);
    Task<Result<LighthouseDto>> GetByIdAsync(Guid lighthouseId);
    Task<Result<Guid>> CreateAsync(LighthouseDto dto);
    Task<Result> UpdateAsync(Guid lighthouseId, LighthouseDto dto);
    Task<Result> DeleteAsync(Guid lighthouseId);
}
