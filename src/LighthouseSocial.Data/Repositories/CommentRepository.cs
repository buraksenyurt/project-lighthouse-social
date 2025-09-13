using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Data.Repositories;

public class CommentRepository(IDbConnectionFactory connFactory, ILogger<CommentRepository> logger)
    : ICommentRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;
    public async Task<Result> AddAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        try
        {
            string sql = @"INSERT INTO comments (id, user_id, photo_id, text, rating, created_at)
                    VALUES (@Id, @UserId, @PhotoId, @Text, @Rating, @CreatedAt);";

            using var conn = _connFactory.CreateConnection();

            var added = await conn.ExecuteAsync(sql, new
            {
                comment.Id,
                comment.UserId,
                comment.PhotoId,
                comment.Text,
                comment.Rating,
                comment.CreatedAt
            });
            return added > 0
                ? Result.Ok()
                : Result.Fail(Messages.Errors.Comment.FailedToAddComment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding comment with Id {CommentId}", comment.Id);
            return Result.Fail($"Exception occured while adding comment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = "DELETE FROM comments WHERE id = @Id;";

            using var conn = _connFactory.CreateConnection();
            var deleted = await conn.ExecuteAsync(sql, new { Id = commentId });
            return deleted > 0
                ? Result.Ok()
                : Result.Fail(Messages.Errors.Comment.FailedToDeleteComment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting comment with Id {CommentId}", commentId);
            return Result.Fail($"Exception occured while deleting comment: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsForUserAsync(Guid userId, Guid photoId, CancellationToken cancellationToken = default)
    {
        try
        {

            const string sql = @"SELECT COUNT(1) FROM comments 
            WHERE user_id = @UserId AND photo_id = @PhotoId;";

            using var conn = _connFactory.CreateConnection();

            var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, PhotoId = photoId });
            return Result<bool>.Ok(count > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existing comment for UserId {UserId} and PhotoId {PhotoId}", userId, photoId);
            return Result<bool>.Fail($"Exception occured while checking existing comment: {ex.Message}");
        }
    }

    public async Task<Result<Comment>> GetByIdAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = "SELECT id, user_id, photo_id, text, rating, created_at FROM comments WHERE id = @Id;";

            using var conn = _connFactory.CreateConnection();

            var comment = await conn.QuerySingleAsync<Comment>(sql, new { Id = commentId });

            if (comment is null)
                return Result<Comment>.Fail(Messages.Errors.Comment.CommentNotFound);

            return Result<Comment>.Ok(comment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving comment with Id {CommentId}", commentId);
            return Result<Comment>.Fail($"Exception occured while retrieving comment: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Comment>>> GetByPhotoIdAsync(Guid photoId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"SELECT id, user_id, photo_id, text, rating, created_at FROM comments 
                            WHERE photo_id = @PhotoId 
                            ORDER BY created_at DESC;";

            using var conn = _connFactory.CreateConnection();

            var comments = await conn.QueryAsync<Comment>(sql, new { PhotoId = photoId });
            return Result<IEnumerable<Comment>>.Ok(comments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving comments for PhotoId {PhotoId}", photoId);
            return Result<IEnumerable<Comment>>.Fail($"Exception occured while retrieving comments: {ex.Message}");
        }
    }
}

