using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts;

public interface ILighthouseService
{
    Task<IEnumerable<LighthouseDto>> GetAllAsync();
    Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto);
    Task<IEnumerable<Photo>> GetPhotosByIdAsync(Guid photoId);
    Task<LighthouseDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(LighthouseDto dto);
    Task<bool> UpdateAsync(Guid id, LighthouseDto dto);
    Task<bool> DeleteAsync(Guid id);
}
