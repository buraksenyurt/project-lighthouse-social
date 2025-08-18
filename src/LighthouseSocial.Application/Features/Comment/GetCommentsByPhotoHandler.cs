using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Comment;
internal record GetCommentsByPhotoRequest(Guid PhotoId);

internal class GetCommentsByPhotoHandler(ICommentRepository repository)
    : IHandler<GetCommentsByPhotoRequest, Result<IEnumerable<CommentDto>>>
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result<IEnumerable<CommentDto>>> HandleAsync(GetCommentsByPhotoRequest request, CancellationToken cancellationToken)
    {
        var comments = await _repository.GetByPhotoIdAsync(request.PhotoId);

        var dtos = comments.Select(c => new CommentDto(c.UserId, c.PhotoId, c.Text, c.Rating.Value));

        return Result<IEnumerable<CommentDto>>.Ok(dtos);
    }
}
