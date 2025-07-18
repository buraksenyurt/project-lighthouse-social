using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Comment;

public class DeleteCommentHandler(ICommentRepository repository)
{
    private readonly ICommentRepository _repository = repository;

    public async Task<Result> HandleAsync(Guid commentId)
    {
        var comment = await _repository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return Result.Fail("Comment not found");
        }

        await _repository.DeleteAsync(commentId);
        return Result.Ok();
    }
}

