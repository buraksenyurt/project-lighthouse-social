using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IPhotoRepository
{
    Task<Result> AddAsync(Photo photo);
    Task<Result<Photo>> GetByIdAsync(Guid id);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<IEnumerable<Photo>>> GetByLighthouseIdAsync(Guid lighthouseId);
    Task<Result<IEnumerable<Photo>>> GetByUserIdAsync(Guid userId);
}
