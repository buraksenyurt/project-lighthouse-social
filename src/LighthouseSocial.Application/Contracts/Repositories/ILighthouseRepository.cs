using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ILighthouseRepository
{
    Task<Result> AddAsync(Lighthouse lighthouse);
    Task<Result> UpdateAsync(Lighthouse lighthouse);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<Lighthouse>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<Lighthouse>>> GetAllAsync();
    Task<Result<IEnumerable<LighthouseWithStats>>> GetTopAsync(int count);
    Task<Result<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)>> GetPagedAsync(int skip, int take);
}    