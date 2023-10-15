using Application.Services.Identity;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            return await ResponseWrapper.FailAsync("Email already taken.");
        }

        var userWithSameUsername = await _userManager.FindByNameAsync(request.UserName);

        if (userWithSameUsername is not null)
        {
            return await ResponseWrapper.FailAsync("Username already taken.");
        }
        
        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.ActivateUser,
            EmailConfirmed = request.AutoConfirmEmail
        };
        
        var password = new PasswordHasher<ApplicationUser>();
        newUser.PasswordHash = password.HashPassword(newUser, request.Password);
        
        var userResult = await _userManager.CreateAsync(newUser);
        
        if (userResult.Succeeded)
        {
            //Assigning a role
            await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
            return await ResponseWrapper<string>.SuccessAsync("User registered successfully.");
        }
        
        return await ResponseWrapper<string>.FailAsync("User registration failed.");
    }
}