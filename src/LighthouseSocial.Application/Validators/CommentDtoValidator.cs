using FluentValidation;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Validators;

public class CommentDtoValidator
    : AbstractValidator<CommentDto>
{
    public CommentDtoValidator()
    {
        RuleFor(o => o.Text)
            .NotEmpty()
            .WithMessage("Comment text cannot be empty")
            .MaximumLength(250)
            .WithMessage("Comment text is too long. Max 250 chars");

        RuleFor(o => o.Rating)
            .InclusiveBetween(1, 10)
            .WithMessage("Rating value must be between 1 and 10");

        RuleFor(o => o.PhotoId).NotEmpty().WithMessage("Invalid Photo Id");
        RuleFor(o => o.UserId).NotEmpty().WithMessage("Invalid User Id");
    }
}
