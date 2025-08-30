using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ICommentRepository
{
    Task<Result> AddAsync(Comment comment);
    Task<Result> DeleteAsync(Guid commentId);
    Task<Result<bool>> ExistsForUserAsync(Guid userId, Guid photoId);
    Task<Result<Comment>> GetByIdAsync(Guid commentId);
    Task<Result<IEnumerable<Comment>>> GetByPhotoIdAsync(Guid photoId);
}
