using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace FluentValidation.AspNetCore.Http.ResultsFactory;
public class SimpleResultsFactory : IFluentValidationEndpointFilterResultsFactory
{
    public IResult Create(ValidationResult validationResult)
    {
        var errors = new Dictionary<string, string[]>();
        var errorByProperty = validationResult.Errors.GroupBy(x => x.PropertyName);
        foreach (var error in errorByProperty)
        {
            errors.Add(error.Key, error.Select(x => x.ErrorMessage).ToArray());
        }
        return TypedResults.ValidationProblem(errors);
    }
}
