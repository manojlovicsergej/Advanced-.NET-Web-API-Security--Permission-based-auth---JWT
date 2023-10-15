using Application.Features.Identity.Commands;
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
}