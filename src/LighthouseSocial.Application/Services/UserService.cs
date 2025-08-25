using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.User;

namespace LighthouseSocial.Application.Services;

public class UserService(PipelineDispatcher pipelineDispatcher)
    : IUserService
{
    public Task<Result<Guid>> CreateUserAsync(UserDto userDto)
    {
        var request = new CreateUserRequest(userDto);
        return pipelineDispatcher.SendAsync<CreateUserRequest, Result<Guid>>(request);
    }

    public Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        var request = new GetUserByIdRequest(userId);
        return pipelineDispatcher.SendAsync<GetUserByIdRequest, Result<UserDto>>(request);
    }

    public Task<Result<UserDto>> GetUserBySubIdAsync(Guid subId)
    {
        var request = new GetUserBySubIdRequest(subId);
        return pipelineDispatcher.SendAsync<GetUserBySubIdRequest, Result<UserDto>>(request);
    }

    public Task<Result<UserDto>> GetUserByEmailAsync(string email)
    {
        var request = new GetUserByEmailRequest(email);
        return pipelineDispatcher.SendAsync<GetUserByEmailRequest, Result<UserDto>>(request);
    }
}
