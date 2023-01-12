using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace FluentValidation.AspNetCore.Http;

/// <summary>
/// Represents a minimal API filter that validates the delegates parameters.
/// </summary>
public class FluentValidationEndpointFilter : IEndpointFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FluentValidationEndpointFilterSettings _settings;
    private readonly ILogger _logger;

    public FluentValidationEndpointFilter(IServiceProvider serviceProvider, FluentValidationEndpointFilterSettings settings, ILogger<FluentValidationEndpointFilter> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Makes the code unclear and harder to read.")]
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogTrace(
            "Validating '{url}' that has {n} arguments with the '{ScanningStrategy}' scanning strategy.",
            context.HttpContext.Request.GetDisplayUrl(),
            context.Arguments.Count,
            _settings.ScanningStrategy
        );
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            var argument = context.Arguments[i];
            if (argument == null)
            {
                _logger.LogDebug("The argument {i} was null. Skipping validation.", i);
                continue;
            }

            var validatorGenericType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorGenericType) as IValidator;
            if (validator is null)
            {
                _logger.LogDebug("No validator found for argument {i}.", i);
                if (_settings.ScanningStrategy == ScanningStrategy.ScanUntilNoValidatorFound)
                {
                    var nextIndex = i + 1;
                    if (context.Arguments.Count != nextIndex)
                    {
                        _logger.LogTrace(
                            "Stopping validation at parameter {i} because of the '{ScanningStrategy}' scanning strategy.",
                            i,
                            _settings.ScanningStrategy
                        );
                    }
                    break;
                }
                continue;
            }

            var contextGenericType = typeof(ValidationContext<>).MakeGenericType(argument.GetType());
            var validationContext = Activator.CreateInstance(contextGenericType, argument) as IValidationContext;
            var results = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (!results.IsValid)
            {
                var errors = new Dictionary<string, string[]>();
                var errorByProperty = results.Errors.GroupBy(x => x.PropertyName);
                foreach (var error in errorByProperty)
                {
                    errors.Add(error.Key, error.Select(x => x.ErrorMessage).ToArray());
                }
                _logger.LogInformation("The validator of argument {i} found {n} errors.", i, errors.Count);
                return HttpResults.ValidationProblem(errors);
            }
        }
        return await next(context);
    }
}
