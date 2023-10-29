using Common.Responses.Identity;

namespace Common.Requests.Identity;

public class UpdateRolePermissionsRequest
{
    public string RoleId { get; set; }
    public List<RoleClaimViewModel> RoleClaims { get; set; }
}