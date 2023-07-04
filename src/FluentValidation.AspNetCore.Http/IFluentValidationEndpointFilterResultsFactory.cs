using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace FluentValidation.AspNetCore.Http;

public interface IFluentValidationEndpointFilterResultsFactory
{
    IResult Create(ValidationResult validationResult);
}