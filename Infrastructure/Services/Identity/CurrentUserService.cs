using System.Security.Claims;
using Application.Services.Identity;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!
            .Value;
    }
    
    public string UserId { get; set; }
}