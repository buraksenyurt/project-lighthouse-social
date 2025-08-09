using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Models;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal class GetLighthouseByIdHandler(ILighthouseRepository repository)
    : IHandler<GetLighthouseByIdRequest, Result<LighthouseDto>>
{
    private readonly ILighthouseRepository _repository = repository;

    public async Task<Result<LighthouseDto>> HandleAsync(GetLighthouseByIdRequest request, CancellationToken cancellationToken)
    {
        var lighthouse = await _repository.GetByIdAsync(request.LighthouseId);

        if (lighthouse == null)
        {
            return Result<LighthouseDto>.Fail("No lighthouse found.");
        }

        var dto = new LighthouseDto(
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            lighthouse.Location.Latitude,
            lighthouse.Location.Longitude
        );

        return Result<LighthouseDto>.Ok(dto);
    }
}
