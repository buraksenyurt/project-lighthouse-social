using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Services;

public class LighthouseService(
    CreateLighthouseHandler createLighthouseHandler, 
    GetAllLighthousesHandler getAllLighthousesHandler, 
    DeleteLighthouseHandler deleteLighthouseHandler, 
    GetLighthouseByIdHandler getLighthouseByIdHandler)
    : ILighthouseService
{
    private readonly CreateLighthouseHandler _createLighthouseHandler = createLighthouseHandler;
    private readonly DeleteLighthouseHandler _deleteLighthouseHandler = deleteLighthouseHandler;
    private readonly GetAllLighthousesHandler _getAllLighthousesHandler = getAllLighthousesHandler;
    private readonly GetLighthouseByIdHandler _getLighthouseByIdHandler = getLighthouseByIdHandler;

    public async Task<Guid> CreateAsync(LighthouseDto dto)
    {
        var result = await _createLighthouseHandler.HandleAsync(dto);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create lighthouse: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public async Task DeleteAsync(Guid id)
    {
        var _ = await _deleteLighthouseHandler.HandleAsync(id);
        //if (!result.Success)
        //{
        //    throw new InvalidOperationException($"Failed to create lighthouse: {result.ErrorMessage}");
        //}
    }

    public async Task<IEnumerable<LighthouseDto>> GetAllAsync()
    {
        var result = await _getAllLighthousesHandler.HandleAsync();
        return result.Success ? result.Data : [];
    }

    public async Task<LighthouseDto?> GetByIdAsync(Guid id)
    {
        var result = await _getLighthouseByIdHandler.HandleAsync(id);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to get lighthouse by id: {result.ErrorMessage}");
        }
        return result.Data;
    }

    public async Task<IEnumerable<Photo>> GetPhotosByIdAsync(Guid photoId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LighthouseDto>> GetTopAsync(TopDto topDto)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Guid id, LighthouseDto dto)
    {
        throw new NotImplementedException();
    }
}
