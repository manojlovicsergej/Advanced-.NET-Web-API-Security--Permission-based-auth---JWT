using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Requests.Employees;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class EmployeesController : MyBaseController<EmployeesController>
{
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest createEmployee)
    {
        var response = await MediatorSender.Send(new CreateEmployeeCommand { CreateEmployeeRequest = createEmployee });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest updateEmployee)
    {
        var response = await MediatorSender.Send(new UpdateEmployeeCommand()
            { UpdateEmployeeRequest = updateEmployee });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployee(int employeeId)
    {
        var response = await MediatorSender.Send(new DeleteEmployeeCommand { EmployeeId = employeeId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetEmployeeList()
    {
        var response = await MediatorSender.Send(new GetEmployeesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetEmployee(int employeeId)
    {
        var response = await MediatorSender.Send(new GetEmployeeByIdQuery{EmployeeId = employeeId});
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
}