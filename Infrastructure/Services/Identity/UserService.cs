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
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICurrentUserService currentUserService, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request,
        CancellationToken cancellationToken)
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

        var identityResult = await _userManager.CreateAsync(newUser);

        if (identityResult.Succeeded)
        {
            //Assigning a role
            await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
            return await ResponseWrapper<string>.SuccessAsync("User registered successfully.");
        }

        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
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

        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request,
        CancellationToken cancellationToken)
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

        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return await ResponseWrapper.FailAsync("User not found!");
        }

        user.IsActive = request.Activate;
        var identityResult = await _userManager.UpdateAsync(user);

        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync(request.Activate
                ? "User activated successfully!"
                : "User de-activated successfully");
        }
        
        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<UserRoleViewModel>();
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return await ResponseWrapper.FailAsync("User does not exist.");    
        }

        var allRoles = await _roleManager.Roles.ToListAsync(cancellationToken);
        foreach (var role in allRoles)
        {
            var userRoleVM = new UserRoleViewModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description
            };
            
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRoleVM.IsAssignedToUser = true;
            }
            
            userRoles.Add(userRoleVM);
        }

        return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRoles);
    }

    public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        if (user.Email == AppCredentials.Email)
        {
            return await ResponseWrapper.FailAsync("User Roles update not permitted.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var rolesToBeAssigned = request.Roles
            .Where(x => x.IsAssignedToUser)
            .ToList();

        var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        
        if (currentLoggedInUser is null)
        {
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        if (await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin))
        {
            var identityResult = await _userManager.RemoveFromRolesAsync(user, roles);
            if (identityResult.Succeeded)
            {
                var idResult = await _userManager.AddToRolesAsync(user, rolesToBeAssigned.Select(x => x.RoleName));
                if (idResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("User Roles Updated Successfully.");
                }

                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(idResult));
            }
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        }
        return await ResponseWrapper.FailAsync("User Roles update not permitted.");
    }

    public async Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("User does not exist.");
        }

        return await ResponseWrapper<UserResponse>.SuccessAsync(_mapper.Map<UserResponse>(user));
    }

    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        var errorDescriptions = new List<string>();
        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);            
        }

        return errorDescriptions;
    }
}