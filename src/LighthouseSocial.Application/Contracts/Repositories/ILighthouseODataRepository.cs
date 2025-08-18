using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ILighthouseODataRepository
{
    IQueryable<QueryableLighthouseDto> GetLighthouses();
}
