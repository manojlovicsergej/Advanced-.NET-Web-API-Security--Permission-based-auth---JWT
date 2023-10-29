using System.Collections.ObjectModel;

namespace Common.Authorization;

public record AppPermission(string Feature, string Action, string Group, string Description, bool IsBasic = false)
{
    public string Name => NameFor(Feature, Action);

    public static string NameFor(string feature, string action) => $"Permissions.{feature}.{action}";
}

public class AppPermissions
{
    private static readonly AppPermission[] _all = new AppPermission[]
    {
        new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
        new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
        new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),
        new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
        
        new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update UserRoles"),
        new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "View UserRoles"),
        
        new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
        new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
        new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),
        new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "View Roles"),
        
        new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),
        new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "View Role  Claims/Permissions"),
        
        new(AppFeature.Employees, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Employees"),
        new(AppFeature.Employees, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Employees"),
        new(AppFeature.Employees, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Employees"),
        new(AppFeature.Employees, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Employees", IsBasic:true),
    };

    public static IReadOnlyList<AppPermission> AdminPermissions { get; } = new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());
    
    public static IReadOnlyList<AppPermission> BasicPermissions { get; } = new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

    public static IReadOnlyList<AppPermission> AllPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all);
}