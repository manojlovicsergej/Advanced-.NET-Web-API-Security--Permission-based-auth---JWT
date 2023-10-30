using Application.Features.Employees.Commands;
using Application.Pipelines;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employees.Validators;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
    {
        RuleFor(x => x.UpdateEmployeeRequest)
            .SetValidator(new UpdateEmployeeRequestValidator(employeeService));
    }
}