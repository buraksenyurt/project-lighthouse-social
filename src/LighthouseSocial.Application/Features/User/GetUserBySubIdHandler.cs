using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.User;

internal record GetUserBySubIdRequest(Guid UserId);

internal class GetUserBySubIdHandler(IUserRepository repository)
    : IHandler<GetUserBySubIdRequest, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserBySubIdRequest request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<UserDto>.Fail("UserId is required");
        }

        var user = await repository.GetBySubIdAsync(request.UserId);
        if (user == null)
        {
            return Result<UserDto>.Fail("User not found");
        }

        var userDto = new UserDto(
            user.Id,
            user.ExternalId,
            user.Fullname,
            user.Email
        );

        return Result<UserDto>.Ok(userDto);
    }
}
