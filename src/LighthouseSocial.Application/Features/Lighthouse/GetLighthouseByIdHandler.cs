using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Lighthouse;
internal record GetLighthouseByIdRequest(Guid LighthouseId);

internal class GetLighthouseByIdHandler(ILighthouseRepository repository)
    : IHandler<GetLighthouseByIdRequest, Result<LighthouseDto>>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result<LighthouseDto>> HandleAsync(GetLighthouseByIdRequest request, CancellationToken cancellationToken)
    {
        var lighthouseResult = await _repository.GetByIdAsync(request.LighthouseId, cancellationToken);

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
