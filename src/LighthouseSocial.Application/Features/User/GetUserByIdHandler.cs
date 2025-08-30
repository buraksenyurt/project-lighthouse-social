using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.User;

internal record GetUserByIdRequest(Guid UserId);

internal class GetUserByIdHandler(IUserRepository repository)
    : IHandler<GetUserByIdRequest, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<UserDto>.Fail("UserId is required");
        }

        var userResult = await repository.GetByIdAsync(request.UserId);
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
