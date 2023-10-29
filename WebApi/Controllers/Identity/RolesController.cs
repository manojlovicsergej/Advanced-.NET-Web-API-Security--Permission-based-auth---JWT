using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class RolesController : MyBaseController<RolesController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Roles, AppAction.Create)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var response = await MediatorSender.Send(new CreateRoleCommand { RoleRequest = request });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet]
    [MustHavePermission(AppFeature.Roles, AppAction.Read)]
    public async Task<IActionResult> GetRoles()
    {
        var response = await MediatorSender.Send(new GetRolesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Roles, AppAction.Update)]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest updateRole)
    {
        var response = await MediatorSender.Send(new UpdateRoleCommand { UpdateRole = updateRole });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet("{roleId}")]
    [MustHavePermission(AppFeature.Roles, AppAction.Read)]
    public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
    {
        var response = await MediatorSender.Send(new GetRoleByIdQuery { RoleId = roleId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpDelete("{roleId}")]
    [MustHavePermission(AppFeature.Roles, AppAction.Delete)]
    public async Task<IActionResult> DeleteRole([FromQuery] string roleId)
    {
        var response = await MediatorSender.Send(new DeleteRoleCommand { RoleId = roleId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpPut("update-permissions")]
    [MustHavePermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsRequest request)
    {
        var response = await MediatorSender.Send(new UpdateRolePermissionsCommand { UpdateRolePermissionsRequest = request });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}