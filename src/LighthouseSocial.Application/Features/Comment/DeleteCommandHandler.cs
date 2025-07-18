using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Comment;

public class DeleteCommandHandler
{
    private readonly ICommentRepository _repository;

    public DeleteCommandHandler(ICommentRepository repository)
    {
        _repository = repository;
    }

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

