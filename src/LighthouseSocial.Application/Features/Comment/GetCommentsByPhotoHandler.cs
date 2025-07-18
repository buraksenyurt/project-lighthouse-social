using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Comment;

public class GetCommentsByPhotoHandler(ICommentRepository repository)
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result<IEnumerable<CommentDto>>> HandleAsync(Guid photoId)
    {
        var comments = await _repository.GetByPhotoIdAsync(photoId);

        var dtos = comments.Select(c => new CommentDto(c.UserId, c.PhotoId, c.Text, c.Rating.Value));

        return Result<IEnumerable<CommentDto>>.Ok(dtos);
    }
}
