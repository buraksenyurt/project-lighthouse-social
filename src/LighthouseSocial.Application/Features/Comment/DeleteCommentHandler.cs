using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;

namespace LighthouseSocial.Application.Features.Comment;
internal record DeleteCommentRequest(Guid CommentId);

internal class DeleteCommentHandler(ICommentRepository repository)
    : IHandler<DeleteCommentRequest, Result>
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result> HandleAsync(DeleteCommentRequest request, CancellationToken cancellationToken)
    {
        var commentResult = await _repository.GetByIdAsync(request.CommentId);
        if (!commentResult.Success)
        {
            return Result.Fail(commentResult.ErrorMessage!);
        }

        var deleteResult = await _repository.DeleteAsync(request.CommentId);
        if (!deleteResult.Success)
        {
            return Result.Fail(deleteResult.ErrorMessage!);
        }

        return Result.Ok();
    }
}

