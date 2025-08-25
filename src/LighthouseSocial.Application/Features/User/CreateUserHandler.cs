using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.User;

internal record CreateUserRequest(UserDto User);

internal class CreateUserHandler(IUserRepository userRepository, IValidator<UserDto> validator)
        : IHandler<CreateUserRequest, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var dto = request.User;
        var validation = validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join(";", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var user = await userRepository.GetByIdAsync(dto.Id);
        if (user != null)
        {
            return Result<Guid>.Fail("User already exists");
        }

        var newUser = new Domain.Entities.User(dto.Id, dto.SubId, dto.Fullname, dto.Email);
        var createdUser = await userRepository.AddAsync(newUser);
        if (!createdUser)
        {
            return Result<Guid>.Fail("Failed to create user");
        }

        return Result<Guid>.Ok(newUser.Id);
    }
}
