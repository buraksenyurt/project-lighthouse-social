using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.ExternalServices;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(UserDto userDto);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);
    Task<Result<UserDto>> GetUserByEmailAsync(string email);
}
