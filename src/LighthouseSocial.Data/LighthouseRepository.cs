using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data;

public class LighthouseRepository
    : ILighthouseRepository
{
    public async Task AddAsync(Lighthouse lighthouse)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Lighthouse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Lighthouse?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Lighthouse lighthouse)
    {
        throw new NotImplementedException();
    }
}
