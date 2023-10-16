using Application.Features.Identity.Commands;
using Application.Features.Identity.Queries;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class UsersController : MyBaseController<UsersController>
{
    [HttpPost]
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
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await MediatorSender.Send(new GetAllUsersQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
}