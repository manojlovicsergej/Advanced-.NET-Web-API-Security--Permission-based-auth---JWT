using Application.Features.Identity.Commands;
using Application.Features.Identity.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class UsersController : MyBaseController<UsersController>
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistration)
    {
        var response = await MediatorSender.Send(new UserRegistrationCommand { userRegistration = userRegistration });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpPut]
    [MustHavePermission(AppFeature.Users,AppAction.Update)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUser)
    {
        var response = await MediatorSender.Send(new UpdateUserCommand() { UpdateUser = updateUser });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpPut("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var response = await MediatorSender.Send(new ChangeUserPasswordCommand() { ChangePasswordRequest = changePasswordRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpPut("change-status")]
    [MustHavePermission(AppFeature.Users,AppAction.Update)]
    public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatus)
    {
        var response = await MediatorSender.Send(new ChangeUserStatusCommand() { ChangeUserStatusRequest = changeUserStatus });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpGet("{userId}")]
    [MustHavePermission(AppFeature.Users,AppAction.Read)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var response = await MediatorSender.Send(new GetUserByIdQuery() { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpGet]
    [MustHavePermission(AppFeature.Users,AppAction.Read)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await MediatorSender.Send(new GetAllUsersQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpGet("roles/{userId}")]
    [MustHavePermission(AppFeature.Users,AppAction.Read)]
    public async Task<IActionResult> GetRoles(string userId)
    {
        var response = await MediatorSender.Send(new GetRolesQuery() { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpPut("user-roles")]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRoles)
    {
        var response = await MediatorSender.Send(new UpdateUserRolesCommand { UpdateUserRoles = updateUserRoles });
        
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}