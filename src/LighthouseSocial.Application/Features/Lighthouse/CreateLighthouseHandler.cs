using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record CreateLighthouseRequest(LighthouseDto Lighthouse);

internal class CreateLighthouseHandler(ILighthouseRepository repository, ICountryDataReader countryDataReader, IValidator<LighthouseDto> validator)
    : IHandler<CreateLighthouseRequest, Result<Guid>>
{
    private readonly ILighthouseRepository _repository = repository;
    private readonly ICountryDataReader _countryDataReader = countryDataReader;
    private readonly IValidator<LighthouseDto> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(CreateLighthouseRequest request, CancellationToken cancellationToken)
    {
        var validation = _validator.Validate(request.Lighthouse);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var countryResult = await _countryDataReader.GetByIdAsync(request.Lighthouse.CountryId, cancellationToken);
        if (!countryResult.Success)
        {
            return Result<Guid>.Fail(countryResult.ErrorMessage!);
        }

        var country = countryResult.Data!;
        var location = new Coordinates(request.Lighthouse.Latitude, request.Lighthouse.Longitude);
        var lighthouse = new Domain.Entities.Lighthouse(request.Lighthouse.Id, request.Lighthouse.Name, country, location);

        var addResult = await _repository.AddAsync(lighthouse, cancellationToken);
        if (!addResult.Success)
        {
            return Result<Guid>.Fail(addResult.ErrorMessage!);
        }

        return Result<Guid>.Ok(lighthouse.Id);
    }
}
