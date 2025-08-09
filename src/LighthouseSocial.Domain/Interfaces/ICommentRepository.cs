using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Domain.Interfaces;

public interface ICommentRepository
{
    Task AddAsync(Comment comment);
    Task<bool> DeleteAsync(Guid commentId);
    Task<bool> ExistsForUserAsync(Guid userId, Guid photoId);
    Task<Comment> GetByIdAsync(Guid commentId);
    Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId);
}
