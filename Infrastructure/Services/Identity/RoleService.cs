using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager,
        IMapper mapper, ApplicationDbContext applicationDbContext)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
        _applicationDbContext = applicationDbContext;
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
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

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

    public async Task<IResponseWrapper> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
        {
            return await ResponseWrapper<string>.FailAsync("Role does not exist.");
        }

        return await ResponseWrapper<RoleResponse>.SuccessAsync(_mapper.Map<RoleResponse>(role));
    }

    public async Task<IResponseWrapper> DeleteRoleAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
        {
            return await ResponseWrapper<string>.FailAsync("Role does not exist.");
        }

        if (role.Name != AppRoles.Admin)
        {
            var allUsers = await _userManager.Users.ToListAsync(cancellationToken);

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    return await ResponseWrapper<string>.FailAsync($"Role: {role.Name} is currently assigned to user.");
                }
            }
        }
        else
        {
            return await ResponseWrapper<string>.FailAsync("Cannot delete Admin role.");
        }

        var identityResult = await _roleManager.DeleteAsync(role);

        if (identityResult.Succeeded)
        {
            return await ResponseWrapper<string>.SuccessAsync("Role successfully deleted.");
        }

        return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> GetPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role is null)
        {
            return await ResponseWrapper<string>.FailAsync("Role not found!");
        }

        var allPermission = AppPermissions.AllPermissions;
        var roleClaimResponse = new RoleClaimResponse
        {
            Role = new()
            {
                Id = roleId,
                Name = role.Name,
                Description = role.Description
            },
            RoleClaims = new()
        };

        var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId, cancellationToken);

        var allPermissionsNames = allPermission.Select(x => x.Name).ToList();
        var currentRoleClaimsValues = currentRoleClaims.Select(x => x.ClaimValue).ToList();

        var currentlyAssignedRoleClaimsNames = allPermissionsNames.Intersect(currentRoleClaimsValues).ToList();

        foreach (var permission in allPermission)
        {
            if (currentlyAssignedRoleClaimsNames.Any(x => x == permission.Name))
            {
                roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel()
                {
                    RoleId = roleId,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group,
                    IsAssignedToRole = true,
                });
            }
            else
            {
                roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel()
                {
                    RoleId = roleId,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group,
                    IsAssignedToRole = false,
                });
            }
        }

        return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
    }

    private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId,
        CancellationToken cancellationToken)
    {
        var claims = await _applicationDbContext.RoleClaims
            .Where(x => x.RoleId == roleId)
            .ToListAsync(cancellationToken);

        if (claims.Any())
        {
            return _mapper.Map<List<RoleClaimViewModel>>(claims);
        }

        return new List<RoleClaimViewModel>();
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