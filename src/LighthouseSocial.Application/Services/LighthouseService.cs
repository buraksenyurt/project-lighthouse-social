using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Services;

public class LighthouseService
    : ILighthouseService
{
    private readonly CreateLighthouseHandler _createLighthouseHandler;
    private readonly GetAllLighthousesHandler _getAllLighthousesHandler;

    public LighthouseService(CreateLighthouseHandler createLighthouseHandler, GetAllLighthousesHandler getAllLighthousesHandler)
    {
        _createLighthouseHandler = createLighthouseHandler;
        _getAllLighthousesHandler = getAllLighthousesHandler;
    }

    public async Task<Guid> CreateAsync(LighthouseDto dto)
    {
        var result = await _createLighthouseHandler.HandleAsync(dto);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create lighthouse: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LighthouseDto>> GetAllAsync()
    {
        var result = await _getAllLighthousesHandler.HandleAsync();
        return result.Success ? result.Data : [];
    }

    public Task<LighthouseDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Photo>> GetPhotosByIdAsync(Guid photoId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, LighthouseDto dto)
    {
        throw new NotImplementedException();
    }
}
