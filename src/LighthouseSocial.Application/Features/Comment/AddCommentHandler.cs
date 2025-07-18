using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Comment;

public class AddCommentHandler(ICommentRepository repository)
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result<Guid>> HandleAsync(CommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            return Result<Guid>.Fail("Comment text cannot be empty");
        }

        var comment = new Domain.Entities.Comment(dto.UserId, dto.PhotoId, dto.Text, Rating.FromValue(dto.Rating));

        await _repository.AddAsync(comment);

        return Result<Guid>.Ok(comment.Id);
    }
}
