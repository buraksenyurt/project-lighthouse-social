using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts;

public interface ILighthouseService
{
    Task<IEnumerable<LighthouseDto>> GetAllAsync();
    Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto);
    Task<IEnumerable<PhotoDto>> GetPhotosByIdAsync(Guid lighthouseId);
    Task<LighthouseDto?> GetByIdAsync(Guid lighthouseId);
    Task<Guid> CreateAsync(LighthouseDto dto);
    Task<bool> UpdateAsync(Guid lighthouseId, LighthouseDto dto);
    Task<bool> DeleteAsync(Guid lighthouseId);
}
