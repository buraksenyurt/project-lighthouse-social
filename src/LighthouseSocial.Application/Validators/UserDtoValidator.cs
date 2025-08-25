using FluentValidation;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Validators;

public class UserDtoValidator 
    : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.SubId)
            .NotEmpty().WithMessage("SubId is required");

        RuleFor(x => x.Fullname)
            .NotEmpty().WithMessage("Fullname is required")
            .MaximumLength(100).WithMessage("Fullname must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");
    }
}
