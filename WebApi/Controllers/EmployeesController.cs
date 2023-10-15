using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Authorization;
using Common.Requests.Employees;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class EmployeesController : MyBaseController<EmployeesController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Employees,AppAction.Create)]
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
    [MustHavePermission(AppFeature.Employees,AppAction.Update)]
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
    [MustHavePermission(AppFeature.Employees,AppAction.Delete)]
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
    [MustHavePermission(AppFeature.Employees,AppAction.Read)]
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
    [MustHavePermission(AppFeature.Employees,AppAction.Read)]
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