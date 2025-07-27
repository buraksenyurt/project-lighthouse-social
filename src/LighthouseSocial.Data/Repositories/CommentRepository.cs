using Dapper;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data.Repositories;

public class CommentRepository(IDbConnectionFactory connFactory)
    : ICommentRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;
    public async Task AddAsync(Comment comment)
    {
        string sql = @"INSERT INTO comments (id, user_id, photo_id, text, rating, created_at)
                    VALUES (@Id, @UserId, @PhotoId, @Text, @Rating, @CreatedAt)";

        using var conn = _connFactory.CreateConnection();

        await conn.ExecuteAsync(sql, new
        {
            comment.Id,
            comment.UserId,
            comment.PhotoId,
            comment.Text,
            comment.Rating,
            comment.CreatedAt
        });
    }

    public async Task DeleteAsync(Guid commentId)
    {
        const string sql = "DELETE FROM comments WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = commentId });
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, Guid photoId)
    {
        const string sql = @"SELECT COUNT(1) FROM comments 
            WHERE user_id = @UserId AND photo_id = @PhotoId";

        using var conn = _connFactory.CreateConnection();
        
        var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, PhotoId = photoId });
        return count > 0;
    }

    public async Task<Comment> GetByIdAsync(Guid commentId)
    {
        const string sql = "SELECT id, user_id, photo_id, text, rating, created_at FROM comments WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();

        return await conn.QuerySingleAsync<Comment>(sql, new { Id = commentId });
    }

    public async Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId)
    {
        const string sql = @"SELECT id, user_id, photo_id, text, rating, created_at FROM comments 
                            WHERE photo_id = @PhotoId 
                            ORDER BY created_at DESC";

        using var conn = _connFactory.CreateConnection();

        return await conn.QueryAsync<Comment>(sql, new { PhotoId = photoId });
    }
}

