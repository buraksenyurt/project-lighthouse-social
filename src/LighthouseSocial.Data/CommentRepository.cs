using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data;

public class CommentRepository
    : ICommentRepository
{
    public async Task AddAsync(Comment comment)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, Guid photoId)
    {
        return false;
    }

    public async Task<Comment> GetByIdAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId)
    {
        throw new NotImplementedException();
    }
}
