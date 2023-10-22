using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    
    public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var existingRole = await _roleManager.FindByNameAsync(request.RoleName);

        if (existingRole is not null)
        {
            return await ResponseWrapper<string>.FailAsync("Role already exists!");
        }

        var identityResult = await _roleManager.CreateAsync(new ApplicationRole
        {
            Name = request.RoleName,
            Description = request.RoleDescription
        });

        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync("Role successfully created!");
        }
        
        return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
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