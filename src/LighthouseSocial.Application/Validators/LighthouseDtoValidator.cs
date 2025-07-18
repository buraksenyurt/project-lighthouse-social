using FluentValidation;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Validators;

public class LighthouseDtoValidator
    : AbstractValidator<LighthouseDto>
{
    public LighthouseDtoValidator()
    {
        RuleFor(o => o.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(25)
            .WithMessage("Name length must be less than 25");

        RuleFor(o => o.CountryId)
            .InclusiveBetween(0, 255)
            .WithMessage("CountryId must be between 0 and 255");

        RuleFor(o => o.Latitude).InclusiveBetween(-90, 90);
        RuleFor(o => o.Longitude).InclusiveBetween(-180, 180);
    }
}
