using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Application.Validators;
using LighthouseSocial.Data;
using LighthouseSocial.Infrastructure.Auditors;

namespace LighthouseSocial.Integration.Tests.Features.Comment;

public class AddCommentHandlerIntegrationTests
{
    private readonly AddCommentHandler _handler;

    public AddCommentHandlerIntegrationTests()
    {
        var validator = new CommentDtoValidator();
        var userRepository = new UserRepository();
        var photoRepository = new PhotoRepository();
        var commentRepository = new CommentRepository();
        var commentAuditor = new ExternalCommentAuditor(new HttpClient());

        _handler = new AddCommentHandler(commentRepository, validator, userRepository, photoRepository, commentAuditor);
    }

    [Fact]
    public async Task HandleAsync_Should_Accept_Appropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        var dto = new CommentDto(Guid.NewGuid(), Guid.NewGuid(), "It's a lovely day.", 7);
        var result = await _handler.HandleAsync(dto);

        Assert.True(result.Success);
    }

        [Fact]
    public async Task HandleAsync_Should_Reject_Inappropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        var dto = new CommentDto(Guid.NewGuid(), Guid.NewGuid(), "I hate you.", 1);
        var result = await _handler.HandleAsync(dto);

        Assert.False(result.Success);
    }
}
