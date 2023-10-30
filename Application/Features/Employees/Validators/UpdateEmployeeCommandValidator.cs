﻿using Application.Features.Employees.Commands;
using Application.Pipelines;
using FluentValidation;

namespace Application.Features.Employees.Validators;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.UpdateEmployeeRequest)
            .SetValidator(new UpdateEmployeeRequestValidator());
    }
}