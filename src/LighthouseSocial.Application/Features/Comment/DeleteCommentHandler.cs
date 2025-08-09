using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Comment;
internal record DeleteCommentRequest(Guid CommentId);

internal class DeleteCommentHandler(ICommentRepository repository)
    : IHandler<DeleteCommentRequest, Result>
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result> HandleAsync(DeleteCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _repository.GetByIdAsync(request.CommentId);
        if (comment == null)
        {
            return Result.Fail("Comment not found");
        }

        var result = await _repository.DeleteAsync(request.CommentId);
        if (!result)
        {
            return Result.Fail("Failed to delete comment");
        }
        return Result.Ok();
    }
}

