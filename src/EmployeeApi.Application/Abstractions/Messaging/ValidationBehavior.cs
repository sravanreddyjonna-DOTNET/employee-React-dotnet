using FluentValidation;
using MediatR;

namespace EmployeeApi.Application.Abstractions.Messaging;

/// <summary>
/// MediatR pipeline behavior that runs every registered FluentValidation
/// validator for a request before its handler executes. This is the
/// cross-cutting-concern building block that makes validation "just work" for
/// any new command/query — nothing in the handler itself has to call a
/// validator (Open/Closed: add a validator, get validation, touch nothing else).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
