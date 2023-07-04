# FluentValidation.AspNetCore.Http

[![Build, Test, and Deploy](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml/badge.svg)](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml)
[![NuGet.org](https://img.shields.io/nuget/vpre/ForEvolve.FluentValidation.AspNetCore.Http)](https://www.nuget.org/packages/ForEvolve.FluentValidation.AspNetCore.Http/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fforevolve%2Ffluentvalidation%2Fshield%2FForEvolve.FluentValidation.AspNetCore.Http%2Flatest)](https://f.feedz.io/forevolve/fluentvalidation/packages/ForEvolve.FluentValidation.AspNetCore.Http/latest/download)

A simple ASP.NET Core 7+ Minimal API integration for [FluentValidation](https://github.com/FluentValidation/FluentValidation) 10+.

The package leverages endpoint filters to trigger FluentValidation `IValidator<T>`. In case of a validation error, the filter returns a `TypedResults.ValidationProblem(errors);`, where the `errors` argument represents the collection of failures. You can customize this behavior by implementing the `IFluentValidationEndpointFilterResultsFactory` interface.

## How to install

```
dotnet add package ForEvolve.FluentValidation.AspNetCore.Http
```

> Pre-release feedz.io URL:
>
> https://f.feedz.io/forevolve/fluentvalidation/nuget/index.json

## How to use

In your `Program.cs` file, you must register the library:

```csharp
builder.AddFluentValidationEndpointFilter();
```

You can then add the validation filter to an endpoint:

```csharp
using FluentValidation.AspNetCore.Http;
//...
app.MapGet("/some-path", (SomeParamToValidate model) => {
    // ...
}).AddFluentValidationFilter();
```

You can also add the validation filter to a group instead, which applies the filter to all of the group's endpoints:

```csharp
using FluentValidation.AspNetCore.Http;
//...
var root = app
    .MapGroup("/")
    .AddFluentValidationFilter()
;
// Then you can register endpoints that will get validated, like:
root.MapGet("/some-path", (SomeParamToValidate model) => {
    //...
});
```

You must register your validators normally. Here is an example that has nothing to do with this library and only uses FluentValidation:

```csharp
// Using the assembly scanning feature
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Manually registering a validator
builder.Services.AddScoped<IValidator<MyEntity>, MyEntityValidator>();
```

## Settings

You can configure the `FluentValidationEndpointFilterSettings` class during the registration as follow:

```csharp
builder.AddFluentValidationEndpointFilter(settings => {
    settings.ScanningStrategy = ScanningStrategy.ScanUntilNoValidatorFound;
});
```

You can also leverage ASP.NET Core `Configure` and `PostConfigure` methods as usual, like this:

```csharp
builder.Services.Configure<FluentValidationEndpointFilterSettings>(options =>
{
    options.ScanningStrategy = ScanningStrategy.ScanUntilNoValidatorFound;
});
```

Finally, you can configure the filter in the `appsettings.json` file under the `FluentValidationEndpointFilter` key, like this:

```json
{
  "FluentValidationEndpointFilter": {
    "ScanningStrategy": "ScanUntilNoValidatorFound"
  }
}
```

### ScanningStrategy

The only configuration at this time is the scanning strategy that contains the following options:

1. `ScanAllParams` (**default behavior**): The filter scans all parameters. For each parameter, it tries to get an `IValidator<T>` instance from the ASP.NET Core container. When it finds one, the filter validates the parameter.
1. `ScanUntilNoValidatorFound`: The filter scans parameters until it does not find a validator for a parameter. When that happens, the validation stops.
   > When using this strategy, you must first add the objects to validate, then add the services or other injected types.

## Examples

You can browse the `examples` directory for usage examples.

## Versioning

The package follows _semantic versioning_ and uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) under the hood to automate versioning based on Git commits.
