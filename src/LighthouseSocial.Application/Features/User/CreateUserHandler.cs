using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Events.User;

namespace LighthouseSocial.Application.Features.User;

internal record CreateUserRequest(UserDto User);

internal class CreateUserHandler(IUserRepository userRepository, IValidator<UserDto> validator, IEventPublisher eventPublisher)
    : IHandler<CreateUserRequest, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var dto = request.User;
        
        var creationRequestedEvent = new UserCreationRequested(
            dto.Id,
            dto.SubId,
            dto.Fullname,
            dto.Email,
            Guid.Empty // todo@buraksenyurt In a real application, this would be the ID of the user making the request
        );
        await eventPublisher.PublishAsync(creationRequestedEvent, cancellationToken);

        var validation = validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join(";", validation.Errors.Select(e => e.ErrorMessage));
            
            var validationFailureEvent = new UserCreationFailed(
                dto.Id,
                dto.SubId,
                dto.Fullname,
                dto.Email,
                "Validation failed",
                errors,
                Guid.Empty // todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );
            await eventPublisher.PublishAsync(validationFailureEvent, cancellationToken);
            
            return Result<Guid>.Fail(errors);
        }

        var userResult = await userRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (userResult.Success)
        {
            var duplicateUserFailureEvent = new UserCreationFailed(
                dto.Id,
                dto.SubId,
                dto.Fullname,
                dto.Email,
                "User already exists",
                "A user with this ID already exists in the system",
                Guid.Empty // todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );
            await eventPublisher.PublishAsync(duplicateUserFailureEvent, cancellationToken);
            
            return Result<Guid>.Fail("User already exists");
        }

        var newUser = new Domain.Entities.User(dto.Id, dto.SubId, dto.Fullname, dto.Email);
        var createdUserResult = await userRepository.AddAsync(newUser, cancellationToken);
        if (!createdUserResult.Success)
        {
            var repositoryFailureEvent = new UserCreationFailed(
                dto.Id,
                dto.SubId,
                dto.Fullname,
                dto.Email,
                "Database save failed",
                createdUserResult.ErrorMessage,
                Guid.Empty // todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );
            await eventPublisher.PublishAsync(repositoryFailureEvent, cancellationToken);
            
            return Result<Guid>.Fail(createdUserResult.ErrorMessage ?? "Failed to create user");
        }

        var userCreatedEvent = new UserCreated(
            newUser.Id,
            newUser.ExternalId,
            newUser.Fullname,
            newUser.Email,
            DateTime.UtcNow
        );
        await eventPublisher.PublishAsync(userCreatedEvent, cancellationToken);

        return Result<Guid>.Ok(newUser.Id);
    }
}
