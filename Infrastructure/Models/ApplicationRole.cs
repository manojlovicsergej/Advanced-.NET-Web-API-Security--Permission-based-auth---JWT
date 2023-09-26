using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
}