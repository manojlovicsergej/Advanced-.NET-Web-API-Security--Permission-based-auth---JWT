using AutoMapper;
using Common.Requests.Employees;
using Domain;

namespace Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<Employee, Employee>();
    }
}