using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands;

public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;

    public DeleteEmployeeCommandHandler(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId,cancellationToken);
        if (employeeInDb is null)
        {
            return await ResponseWrapper.FailAsync("Employee does not exist.");
        }
        
        var employeeId = await _employeeService.DeleteEmployeeAsync(employeeInDb,cancellationToken);
        return await ResponseWrapper<int>.SuccessAsync(employeeId, "Employee entry deleted successfully.");
    }
}