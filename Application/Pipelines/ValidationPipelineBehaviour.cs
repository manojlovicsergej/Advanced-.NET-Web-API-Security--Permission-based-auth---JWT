using Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Pipelines;

public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidateMe
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehaviour()
    {
        _validators = _validators;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        List<string> errors = new();
        var validationResults = await Task
            .WhenAll(_validators
                .Select(x => x.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(x => x.Errors)
            .Where(z => z != null)
            .ToList();

        foreach (var failure in failures)
        {
            errors.Add(failure.ErrorMessage);
        }

        throw new CustomValidationException(errors, "One or more validation failure(s) occured.");
    }
}