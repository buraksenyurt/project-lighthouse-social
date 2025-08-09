using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<bool> AddAsync(Photo photo);
    Task<Photo?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Photo>> GetByLighthouseIdAsync(Guid lighthouseId);
    Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId);
}
