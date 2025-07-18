using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Domain.Interfaces;

public interface ICommentRepository
{
    Task AddAsync(Comment comment);
    Task DeleteAsync(Guid commentId);
    Task<Comment> GetByIdAsync(Guid commentId);
    Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId);
}
