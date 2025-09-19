using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Application.Validators;
using LighthouseSocial.Data;
using LighthouseSocial.Data.Repositories;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.Messaging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LighthouseSocial.Integration.Tests.Features.Comment;

public class AddCommentHandlerIntegrationTests
{
    private readonly AddCommentHandler _handler;

    public AddCommentHandlerIntegrationTests()
    {
        var validator = new CommentDtoValidator();
        var connectionString = "Host=localhost;Port=5432;Database=lighthousedb;Username=johndoe;Password=somew0rds";
        var factory = new NpgsqlConnectionFactory(connectionString);

        var commentLogger = NullLogger<CommentRepository>.Instance;
        var userLogger = NullLogger<UserRepository>.Instance;
        var photoLogger = NullLogger<PhotoRepository>.Instance;
        var commentAuditorLogger = NullLogger<ExternalCommentAuditor>.Instance;
        var eventPublisherLogger = NullLogger<RabbitMqEventPublisher>.Instance;

        var userRepository = new UserRepository(factory, userLogger);
        var photoRepository = new PhotoRepository(factory, photoLogger);
        var commentRepository = new CommentRepository(factory, commentLogger);
        var commentAuditor = new ExternalCommentAuditor(new HttpClient(), commentAuditorLogger);

        var rabbitMqSettings = new RabbitMqSettings
        {
            UserName = "admin",
            Password = "admin1234",
            HostName = "localhost",
            Port = 5672,
            VirtualHost = "/",
            ExchangeName = "lighthouse-exchange",
            QueueName = "lighthouse-queue",
            Durable = true,
            AutoDelete = false
        };
        var options = Microsoft.Extensions.Options.Options.Create(rabbitMqSettings);

        var eventPublisher = new RabbitMqEventPublisher(options, eventPublisherLogger);

        _handler = new AddCommentHandler(commentRepository, validator, userRepository, photoRepository, commentAuditor, eventPublisher);
    }

    [Fact]
    public async Task HandleAsync_Should_Accept_Appropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        var dto = new CommentDto(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("20000000-0000-0000-0000-000000000001"), "It's a lovely day.", 7);
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task HandleAsync_Should_Reject_Inappropriate_Comment()
    {
        if (Environment.GetEnvironmentVariable("CI") == "true")
            return;

        var dto = new CommentDto(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("20000000-0000-0000-0000-000000000001"), "I hate you.", 1);
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        Assert.False(result.Success);
    }
}
