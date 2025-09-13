using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ICommentRepository
{
    Task<Result> AddAsync(Comment comment, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsForUserAsync(Guid userId, Guid photoId, CancellationToken cancellationToken = default);
    Task<Result<Comment>> GetByIdAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Comment>>> GetByPhotoIdAsync(Guid photoId, CancellationToken cancellationToken = default);
}
