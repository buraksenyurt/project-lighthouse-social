using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Events.Lighthouse;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Lighthouse;

internal record CreateLighthouseRequest(LighthouseDto Lighthouse);

internal class CreateLighthouseHandler(ILighthouseRepository repository, ICountryDataReader countryDataReader, IValidator<LighthouseDto> validator, IEventPublisher eventPublisher)
    : IHandler<CreateLighthouseRequest, Result<Guid>>
{
    private readonly ILighthouseRepository _repository = repository;
    private readonly ICountryDataReader _countryDataReader = countryDataReader;
    private readonly IValidator<LighthouseDto> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(CreateLighthouseRequest request, CancellationToken cancellationToken)
    {
        var creationRequestedEvent = new LighthouseCreationRequested(
            request.Lighthouse.Id,
            request.Lighthouse.Name,
            request.Lighthouse.CountryId,
            request.Lighthouse.Latitude,
            request.Lighthouse.Longitude,
            Guid.Empty //todo@buraksenyurt In a real application, this would be the ID of the user making the request
        );
        await eventPublisher.PublishAsync(creationRequestedEvent, cancellationToken);


        var validation = _validator.Validate(request.Lighthouse);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));

            var failureEvent = new LighthouseCreationFailed(
                request.Lighthouse.Id,
                request.Lighthouse.Name,
                request.Lighthouse.CountryId,
                request.Lighthouse.Latitude,
                request.Lighthouse.Longitude,
                "Validation failed",
                errors,
                Guid.Empty //todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );
            await eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(errors);
        }

        var countryResult = await _countryDataReader.GetByIdAsync(request.Lighthouse.CountryId, cancellationToken);
        if (!countryResult.Success)
        {
            var failureEvent = new LighthouseCreationFailed(
                request.Lighthouse.Id,
                request.Lighthouse.Name,
                request.Lighthouse.CountryId,
                request.Lighthouse.Latitude,
                request.Lighthouse.Longitude,
                "Country not found",
                countryResult.ErrorMessage,
                Guid.Empty //todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );
            await eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(countryResult.ErrorMessage!);
        }

        var country = countryResult.Data!;
        var location = new Coordinates(request.Lighthouse.Latitude, request.Lighthouse.Longitude);
        var lighthouse = new Domain.Entities.Lighthouse(request.Lighthouse.Id, request.Lighthouse.Name, country, location);

        var addResult = await _repository.AddAsync(lighthouse, cancellationToken);
        if (!addResult.Success)
        {
            var failureEvent = new LighthouseCreationFailed(
                request.Lighthouse.Id,
                request.Lighthouse.Name,
                request.Lighthouse.CountryId,
                request.Lighthouse.Latitude,
                request.Lighthouse.Longitude,
                "Database save failed:",
                addResult.ErrorMessage,
                Guid.Empty //todo@buraksenyurt In a real application, this would be the ID of the user making the request
            );

            return Result<Guid>.Fail(addResult.ErrorMessage!);
        }

        var successEvent = new LighthouseCreated(
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            lighthouse.Country.Name,
            lighthouse.Location.Latitude,
            lighthouse.Location.Longitude,
            DateTime.UtcNow
        );
        await eventPublisher.PublishAsync(successEvent, cancellationToken);

        return Result<Guid>.Ok(lighthouse.Id);
    }
}
