# FluentValidation.AspNetCore.Http

[![Build, Test, and Deploy](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml/badge.svg)](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml)
[![NuGet.org](https://img.shields.io/nuget/vpre/ForEvolve.FluentValidation.AspNetCore.Http)](https://www.nuget.org/packages/ForEvolve.FluentValidation.AspNetCore.Http/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fforevolve%2Ffluentvalidation%2Fshield%2FForEvolve.FluentValidation.AspNetCore.Http%2Flatest)](https://f.feedz.io/forevolve/fluentvalidation/packages/ForEvolve.FluentValidation.AspNetCore.Http/latest/download)

An ASP.NET Core 7+ Minimal API integration for [FluentValidation](https://github.com/FluentValidation/FluentValidation) 10+.

The package adds an `IEndpointFilter` that triggers FluentValidation's `IValidator<T>` implementations. In case of a validation error, the filter returns a `HttpResults.ValidationProblem(errors);`, where the `errors` argument represents the collection of failures.

> Work in progress...

## How to install

```
dotnet add package ForEvolve.FluentValidation.AspNetCore.Http
```

> You can use the following pre-release feed URL:
>
> https://f.feedz.io/forevolve/fluentvalidation/nuget/index.json

### Versioning

The package follows _semantic versioning_ and uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) under the hood to automate versioning based on Git commits.

## How to use

In your `Program.cs` file, register your validators normally. Here is an example:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

Then, you must register the filter's settings, like this:

```csharp
builder.AddFluentValidationEndpointFilter();
```

Once the registration is completed, you can add the filter to an endpoint like this:

```csharp
using FluentValidation.AspNetCore.Http;
//...
app.MapGet("/some-path", (SomeParamToValidate model) => {
    // ...
}).AddEndpointFilter<FluentValidationEndpointFilter>();
```

You can also add the filter to a group instead, which will apply it to all of its endpoints, like this:

```csharp
using FluentValidation.AspNetCore.Http;
//...
var rootGroup = app
    .MapGroup("/")
    .AddEndpointFilter<FluentValidationEndpointFilter>()
;
// Then you can register endpoints that will get validated, like:
rootGroup.MapGet("/some-path", (SomeParamToValidate model) => {
    //...
});
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
// TODO
```

Finally, you can configure the filter in the `appsettings.json` file under the `FluentValidationEndpointFilter` key, like this:

```json
{
  "FluentValidationEndpointFilter": {
    // TODO
  }
}
```

### ScanningStrategy

The only configuration at this time is the scanning strategy.

1. `ScanAllParams` (**default behavior**): The filter scans all parameters. For each parameter, it tries to get an `IValidator<T>` instance from the ASP.NET Core container. When it finds one, the filter validates the parameter.
1. `ScanUntilNoValidatorFound`: The filter scans parameters until it does not find a validator for a parameter. When that happens, the validation stops.
   > When using this strategy, you must first add the objects to validate, then add the services or other injected types.
