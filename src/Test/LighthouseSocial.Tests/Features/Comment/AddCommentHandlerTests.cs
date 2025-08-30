using FluentValidation;
using FluentValidation.Results;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using Moq;

namespace LighthouseSocial.Tests.Features.Comment;

public class AddCommentHandlerTests
{
    private readonly Mock<ICommentRepository> _repositoryMock;
    private readonly Mock<IValidator<CommentDto>> _validatorMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPhotoRepository> _photoRepositoryMock;
    private readonly Mock<ICommentAuditor> _commentAuditorMock;
    private readonly AddCommentHandler _handler;
    public AddCommentHandlerTests()
    {
        _repositoryMock = new Mock<ICommentRepository>();
        _validatorMock = new Mock<IValidator<CommentDto>>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _photoRepositoryMock = new Mock<IPhotoRepository>();
        _commentAuditorMock = new Mock<ICommentAuditor>();

        _handler = new AddCommentHandler(
            _repositoryMock.Object,
            _validatorMock.Object,
            _userRepositoryMock.Object,
            _photoRepositoryMock.Object,
            _commentAuditorMock.Object
            );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        var dto = new CommentDto(Guid.NewGuid(), Guid.NewGuid(), "Lovely photo", 5);

        _validatorMock.Setup(v => v.Validate(It.IsAny<CommentDto>())).Returns(new ValidationResult());
        _userRepositoryMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>()))
       .ReturnsAsync(new Domain.Entities.User(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), "tester"));

        _photoRepositoryMock.Setup(p => p.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Domain.Entities.Photo>.Ok(new Domain.Entities.Photo(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "EndOfTheWorld.jpg",
                new Domain.ValueObjects.PhotoMetadata("50mm", "1280x1280", "Canon Mark 5", DateTime.Now.AddDays(-7))
            )));

        _repositoryMock.Setup(r => r.ExistsForUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        _commentAuditorMock.Setup(a => a.IsTextCleanAsync(dto.Text)).ReturnsAsync(true);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Comment>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Data);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Comment>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        var dto = new CommentDto(Guid.Empty, Guid.Empty, string.Empty, 11);
        var validationFailures = new List<ValidationFailure>
        {
            new("Text","Comment text cannot be empty"),
            new("Rating","Rating value must be between 1 and 10"),
            new("PhotoId","Invalid Photo Id"),
            new("UserId","Invalid User Id")
        };

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<CommentDto>()))
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.HandleAsync(new AddCommentRequest(dto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Comment text cannot be empty", result.ErrorMessage);
        Assert.Contains("Rating value must be between 1 and 10", result.ErrorMessage);
        Assert.Contains("Invalid Photo Id", result.ErrorMessage);
        Assert.Contains("Invalid User Id", result.ErrorMessage);
    }
}
