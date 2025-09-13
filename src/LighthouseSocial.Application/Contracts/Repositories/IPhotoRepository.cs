using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IPhotoRepository
{
    Task<Result> AddAsync(Photo photo, CancellationToken cancellationToken = default);
    Task<Result<Photo>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Photo>>> GetByLighthouseIdAsync(Guid lighthouseId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Photo>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
