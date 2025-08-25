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
}
