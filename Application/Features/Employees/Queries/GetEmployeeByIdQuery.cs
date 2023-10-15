using Application.Services;
using AutoMapper;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Queries;

public class GetEmployeeByIdQuery : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public GetEmployeeByIdQueryHandler(IEmployeeService employeeService, IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId,cancellationToken);
        if (employeeInDb is null)
        {
            return await ResponseWrapper.FailAsync("Employee does not exist.");
        }

        var mappedEmployee = _mapper.Map<EmployeeResponse>(employeeInDb);
        return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedEmployee);
    }
}