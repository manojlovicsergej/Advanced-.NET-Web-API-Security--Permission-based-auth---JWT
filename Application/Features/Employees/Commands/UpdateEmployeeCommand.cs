using Application.Services;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Employees.Commands;

public class UpdateEmployeeCommand : IRequest<IResponseWrapper>
{
    public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }
    
    public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeInDb =
            await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id, cancellationToken);

        if (employeeInDb is null)
        {
            return await ResponseWrapper.FailAsync("Employee does not exist.");
        }

        employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
        employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
        employeeInDb.Email = request.UpdateEmployeeRequest.Email;
        employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;

        var updateEmployee = await _employeeService.UpdateEmployeeAsync(employeeInDb, cancellationToken);
        return await ResponseWrapper<EmployeeResponse>.SuccessAsync(_mapper.Map<EmployeeResponse>(updateEmployee), "Employee updated successfully.");
    }
}