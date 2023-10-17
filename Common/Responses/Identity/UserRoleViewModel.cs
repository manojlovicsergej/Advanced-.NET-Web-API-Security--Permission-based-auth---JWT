namespace Common.Responses.Identity;

public class UserRoleViewModel
{
    public string RoleName { get; set; }
    public string RoleDescription { get; set; }
    public bool IsAssignedToUser { get; set; }
}