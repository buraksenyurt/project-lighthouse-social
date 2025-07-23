using FluentValidation;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Enumerations;

namespace LighthouseSocial.Application.Validators;

public class PhotoDtoValidator
    : AbstractValidator<PhotoDto>
{
    public PhotoDtoValidator()
    {
        RuleFor(o => o.FileName)
            .NotEmpty().WithMessage("Filename can not be empty.")
            .MaximumLength(50).WithMessage("Filename is too long.")
            .Matches(@"\.(jpg|jpeg|png|gif)$")
            .WithMessage("Only image files are allowed (.jpg, .jpeg, .png, .gif).");

        RuleFor(o => o.UploadedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Upload date must be in the past or now.");

        RuleFor(o => o.CameraType)
            .NotEmpty().WithMessage("Camera model is required.")
            // Domain katmanındaki kamera türlerinden(SLR,DSLR,Phone,Mirrorless) biri değilse
            .Must(BeValidCameraType).WithMessage("Camera type is not recognized");

        RuleFor(o => o.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be valid.");

        RuleFor(o => o.LighthouseId)
            .NotEmpty().WithMessage("LighthouseId is required.")
            .NotEqual(Guid.Empty).WithMessage("LighthouseId must be valid.");
    }

    private bool BeValidCameraType(string input)
    {
        try
        {
            _ = CameraType.FromName(input); // Domain katmanındaki kamera türlerinden kontrol ediyoruz
            return true;
        }
        catch
        {
            return false;
        }
    }
}
