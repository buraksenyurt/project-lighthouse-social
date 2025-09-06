using FluentValidation;
using FluentValidation.Results;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.User;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.User;

public class CreateUserHandlerTests
{
    private readonly Mock<IValidator<UserDto>> _validatorMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _validatorMock = new Mock<IValidator<UserDto>>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserHandler(_userRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        var dto = new UserDto(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "john.doe@plhsocial.com");

        _validatorMock.Setup(v => v.Validate(It.IsAny<UserDto>())).Returns(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(Result<Domain.Entities.User>.Fail("User not found"));
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.User>())).ReturnsAsync(Result.Ok());

        // Act
        var result = await _handler.HandleAsync(new CreateUserRequest(dto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(dto.Id, result.Data);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var dto = new UserDto(Guid.NewGuid(), Guid.NewGuid(), "", "invalid-email");
        var validationFailures = new List<ValidationFailure>
        {
            new("Fullname", "Fullname is required"),
            new("Email", "Email is not valid")
        };
        _validatorMock.Setup(v => v.Validate(It.IsAny<UserDto>())).Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.HandleAsync(new CreateUserRequest(dto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Fullname is required", result.ErrorMessage);
        Assert.Contains("Email is not valid", result.ErrorMessage);
        _validatorMock.Verify(v => v.Validate(dto), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = new UserDto(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "john.doe@plhsocial.com");
        _validatorMock.Setup(v => v.Validate(It.IsAny<UserDto>())).Returns(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Domain.Entities.User>.Ok(new Domain.Entities.User(Guid.NewGuid(), Guid.NewGuid(), "Existing User", "")));

        // Act
        var result = await _handler.HandleAsync(new CreateUserRequest(dto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User already exists", result.ErrorMessage);
        _validatorMock.Verify(v => v.Validate(dto), Times.Once);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}