using AutoMapper;
using Common.Responses.Identity;
using Infrastructure.Models;

namespace Infrastructure;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ApplicationUser, UserResponse>();
        CreateMap<ApplicationRole, RoleResponse>();
    }
}