using Common.Authorization;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ApplicationDbSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbSeeder(ApplicationDbContext dbContext, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedDatabaseAsync()
    {
        // Check for pending and apply if any
        await CheckAndApplyPendingMigrationAsync();
        // Seed roles
        await SeedRolesAsync();
        // Seed user (Admin)
        await SeedAdminUserAsync();
    }

    private async Task CheckAndApplyPendingMigrationAsync()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleName) 
                is not ApplicationRole role)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role."
                };

                await _roleManager.CreateAsync(role);
            }
            
            //Assign permissions
            switch (roleName)
            {
                case AppRoles.Admin:
                    await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
                    break;
                case AppRoles.Basic:
                    await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
                    break;
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role, IEnumerable<AppPermission> permissions)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
            {
                await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminUserName = AppCredentials.Email[..AppCredentials.Email.IndexOf('@')].ToLowerInvariant();
        var adminUser = new ApplicationUser
        {
            FirstName = "Sergej",
            LastName = "Manojlovic",
            Email = AppCredentials.Email,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
            NormalizedUserName = adminUserName.ToUpperInvariant(),
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(x => x.Email == AppCredentials.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }
        
        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) &&
            !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
        }
    }
}