using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface ILighthouseRepository
{
    Task<Result> AddAsync(Lighthouse lighthouse, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Lighthouse lighthouse, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Lighthouse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Lighthouse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LighthouseWithStats>>> GetTopAsync(int count, CancellationToken cancellationToken = default);
    Task<Result<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);
}