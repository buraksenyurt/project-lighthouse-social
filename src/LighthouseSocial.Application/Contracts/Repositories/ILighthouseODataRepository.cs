using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ILighthouseODataRepository
{
    Task<Result<IQueryable<QueryableLighthouseDto>>> GetLighthousesAsync();
}
