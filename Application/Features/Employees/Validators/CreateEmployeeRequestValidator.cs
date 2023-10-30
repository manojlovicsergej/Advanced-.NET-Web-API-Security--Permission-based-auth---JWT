using Common.Requests.Employees;
using FluentValidation;

namespace Application.Features.Employees.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Employee firstname is required.")
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Employee lastname is required.")
            .MaximumLength(60);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Employee email is required.")
            .MaximumLength(100);

        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Employee must have salary.");
    }
}