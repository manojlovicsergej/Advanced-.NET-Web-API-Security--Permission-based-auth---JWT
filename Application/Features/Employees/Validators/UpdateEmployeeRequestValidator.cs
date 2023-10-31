using Application.Services;
using Common.Requests.Employees;
using Domain;
using FluentValidation;

namespace Application.Features.Employees.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(IEmployeeService employeeService)
    {
        RuleFor(request => request.Id)
            .MustAsync(async (id, ct) => await employeeService.GetEmployeeByIdAsync(id, ct)
                is Employee employee && employee.Id == id)
            .WithMessage("Employee does not exist.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Employee firstname is required.")
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Employee lastname is required.")
            .MaximumLength(60);

        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty().WithMessage("Employee email is required.")
            .MaximumLength(100);

        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Employee must have salary.");
    }
}