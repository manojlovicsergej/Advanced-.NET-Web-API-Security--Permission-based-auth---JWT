using Application.Features.Identity.Commands;
using Application.Services.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators;

public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
{
    public UserRegistrationCommandValidator(IUserService userService)
    {
        RuleFor(command => command.userRegistration)
            .SetValidator(new UserRegistrationRequestValidator(userService));
    }
}