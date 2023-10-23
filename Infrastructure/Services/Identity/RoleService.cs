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

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
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

    public async Task<IResponseWrapper> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles =  await _roleManager.Roles.ToListAsync(cancellationToken);

        if (!roles.Any())
        {
            return await ResponseWrapper<string>.FailAsync("No roles found!");
        }
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(_mapper.Map<List<RoleResponse>>(roles));
    }

    public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);
        if (role is null || role.Name == AppRoles.Admin)
        {
            return await ResponseWrapper<string>.FailAsync("Role does not exist.");
        }

        role.Name = request.RoleName;
        role.Description = request.RoleDescription;

        var identityResult = await _roleManager.UpdateAsync(role);
        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync("Role updated successfully!");
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