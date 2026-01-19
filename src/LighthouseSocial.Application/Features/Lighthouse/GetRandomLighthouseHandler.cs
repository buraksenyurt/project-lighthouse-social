using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record GetRandomLighthouseRequest();

internal class GetRandomLighthouseHandler(ILighthouseRepository repository)
    : IHandler<GetRandomLighthouseRequest, Result<LighthouseDto>>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result<LighthouseDto>> HandleAsync(GetRandomLighthouseRequest request, CancellationToken cancellationToken)
    {
        var lighthouseResult = await _repository.GetRandomAsync(cancellationToken);

        if (!lighthouseResult.Success)
        {
            return Result<LighthouseDto>.Fail(lighthouseResult.ErrorMessage!);
        }

        var lighthouse = lighthouseResult.Data!;

        var dto = new LighthouseDto(
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            lighthouse.Country.Name,
            lighthouse.Location.Latitude,
            lighthouse.Location.Longitude
        );

        return Result<LighthouseDto>.Ok(dto);
    }
}
