using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators;

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationRequestValidator(IUserService userService)
    {
        RuleFor(request => request.Email)
            .MustAsync(async (email, ct) => await userService.GetUserByEmailAsync(email, ct)
                is not  UserResponse user)
            .WithMessage("Email already taken.");
        
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(60);
        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(60);
        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(request => request.UserName)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(request => request.ConfirmPassword)
            .Must((req, confirmed) => req.Password == confirmed)
            .WithMessage("Passwords does not match");
    }
}