using Common.Requests.Employees;
using FluentValidation;

namespace Application.Features.Employees.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(60);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Salary)
            .NotEmpty();
    }
}