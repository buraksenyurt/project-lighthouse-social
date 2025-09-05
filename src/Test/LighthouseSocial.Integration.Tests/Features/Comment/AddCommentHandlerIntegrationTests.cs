using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Application.Validators;
using LighthouseSocial.Data;
using LighthouseSocial.Data.Repositories;
using LighthouseSocial.Infrastructure.Auditors;
using Microsoft.Extensions.Logging.Abstractions;

namespace LighthouseSocial.Integration.Tests.Features.Comment;

public class AddCommentHandlerIntegrationTests
{
    private readonly AddCommentHandler _handler;

    public AddCommentHandlerIntegrationTests()
    {
        var validator = new CommentDtoValidator();
        //todo@buraksenyurt Entegrasyon testi de olsa Connection bilgisi Secure Vault üstünden gelmeli
        var connectionString = "Host=localhost;Port=5432;Database=lighthousedb;Username=johndoe;Password=somew0rds";
        var factory = new NpgsqlConnectionFactory(connectionString);

        var userRepository = new UserRepository(factory);
        var photoRepository = new PhotoRepository(factory);
        var commentLogger = NullLogger<CommentRepository>.Instance;
        var commentRepository = new CommentRepository(factory, commentLogger);
        var commentAuditor = new ExternalCommentAuditor(new HttpClient());

        _handler = new AddCommentHandler(commentRepository, validator, userRepository, photoRepository, commentAuditor);
    }

    [Fact]
    public async Task HandleAsync_Should_Accept_Appropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        //todo@buraksenyurt Sistemde var olan UserId, PhotoId bilgileri ile test edilebilir
        var dto = new CommentDto(Guid.NewGuid(), Guid.NewGuid(), "It's a lovely day.", 7);
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task HandleAsync_Should_Reject_Inappropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        //todo@buraksenyurt Sistemde var olan UserId, PhotoId bilgileri ile test edilebilir
        var dto = new CommentDto(Guid.NewGuid(), Guid.NewGuid(), "I hate you.", 1);
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        Assert.False(result.Success);
    }
}
