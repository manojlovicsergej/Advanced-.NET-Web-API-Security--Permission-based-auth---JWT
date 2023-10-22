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
    [MustHavePermission(AppFeature.Roles,AppAction.Create)]
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
    [MustHavePermission(AppFeature.Roles,AppAction.Read)]
    public async Task<IActionResult> GetRoles()
    {
        var response = await MediatorSender.Send(new GetRolesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
}