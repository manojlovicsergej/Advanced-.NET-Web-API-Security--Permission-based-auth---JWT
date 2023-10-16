using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
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

    public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        { 
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        return await ResponseWrapper<UserResponse>.SuccessAsync(_mapper.Map<UserResponse>(user));
    }

    public async Task<IResponseWrapper> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);

        if (!users.Any())
        {
            return await ResponseWrapper.FailAsync("No users were found!");
        }

        return await ResponseWrapper<List<UserResponse>>.SuccessAsync(_mapper.Map<List<UserResponse>>(users));
    }

    public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return await ResponseWrapper.FailAsync("User not found!");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var identityResult = await _userManager.UpdateAsync(user);
        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync("User details successfully updated!");
        }

        return await ResponseWrapper.FailAsync("Failed to update user details.");
    }

    public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return await ResponseWrapper.FailAsync("User not found!");
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync("User password successfully updated!");
        }
        
        return await ResponseWrapper.FailAsync("Failed to update user password.");
    }
}