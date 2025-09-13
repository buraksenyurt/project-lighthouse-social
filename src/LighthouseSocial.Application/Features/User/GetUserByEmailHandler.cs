using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.User;

internal record GetUserByEmailRequest(string Email);
internal class GetUserByEmailHandler(IUserRepository repository)
    : IHandler<GetUserByEmailRequest, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserByEmailRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<UserDto>.Fail("Email is required");
        }

        var userResult = await repository.GetByEmailAsync(request.Email, cancellationToken);
        if (!userResult.Success)
        {
            return Result<UserDto>.Fail(userResult.ErrorMessage ?? "User not found");
        }
        var user = userResult.Data!;
        var userDto = new UserDto(
            user.Id,
            user.ExternalId,
            user.Fullname,
            user.Email
        );

        return Result<UserDto>.Ok(userDto);
    }
}
