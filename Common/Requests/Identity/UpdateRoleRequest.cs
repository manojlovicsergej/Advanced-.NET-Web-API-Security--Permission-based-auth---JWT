namespace Common.Requests.Identity;

public class UpdateRoleRequest
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public string RoleDescription { get; set; }
}