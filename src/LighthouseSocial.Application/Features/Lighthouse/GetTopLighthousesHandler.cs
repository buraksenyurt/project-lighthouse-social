using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record GetTopLighthousesRequest(int Count);

internal class GetTopLighthousesHandler(ILighthouseRepository repository)
    : IHandler<GetTopLighthousesRequest, Result<IEnumerable<LighthouseTopDto>>>
{
    public async Task<Result<IEnumerable<LighthouseTopDto>>> HandleAsync(GetTopLighthousesRequest request, CancellationToken cancellationToken)
    {
        var statsResult = await repository.GetTopAsync(request.Count);
        
        if (!statsResult.Success)
        {
            return Result<IEnumerable<LighthouseTopDto>>.Fail(statsResult.ErrorMessage!);
        }

        var stats = statsResult.Data!;
        
        if (!stats.Any())
        {
            return Result<IEnumerable<LighthouseTopDto>>.Fail(Messages.Errors.Lighthouse.NoLighthousesFound);
        }

        var dtos = stats.Select(s => new LighthouseTopDto
        {
            Id = s.Id,
            Name = s.Name,
            PhotoCount = s.PhotoCount,
            AverageScore = s.AverageScore
        });

        return Result<IEnumerable<LighthouseTopDto>>.Ok(dtos);
    }
}
