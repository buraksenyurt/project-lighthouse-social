using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ILighthouseRepository
{
    Task<bool> AddAsync(Lighthouse lighthouse);
    Task<bool> UpdateAsync(Lighthouse lighthouse);
    Task<bool> DeleteAsync(Guid id);
    Task<Lighthouse?> GetByIdAsync(Guid id);
    Task<IEnumerable<Lighthouse>> GetAllAsync();
    Task<IEnumerable<LighthouseWithStats>> GetTopAsync(int count);
    Task<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)> GetPagedAsync(int skip, int take);
}
    