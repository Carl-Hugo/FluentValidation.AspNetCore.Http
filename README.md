# FluentValidation.AspNetCore.Http

[![Build, Test, and Deploy](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml/badge.svg)](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http/actions/workflows/main.yml)
[![NuGet.org](https://img.shields.io/nuget/vpre/FluentValidation.AspNetCore.Http)](https://www.nuget.org/packages/FluentValidation.AspNetCore.Http/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fforevolve%2Ffluentvalidation%2Fshield%2FFluentValidation.AspNetCore.Http%2Flatest)](https://f.feedz.io/forevolve/fluentvalidation/packages/FluentValidation.AspNetCore.Http/latest/download)

An ASP.NET Core 7+ Minimal API integration for [FluentValidation](https://github.com/FluentValidation/FluentValidation).

The package adds an `IEndpointFilter` that triggers FluentValidation's `IValidator<T>` implementations. In case of a validation error, the filter returns a `HttpResults.ValidationProblem(errors);`, where the `errors` argument represents the collection of failures.

> Work in progress...

## How to install

```
dotnet add package FluentValidation.AspNetCore.Http
```

> The pre-release feed URL is the following:
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
app.MapGet("/some-path", (SomeParamToValidate model) => {
    // ...
}).AddEndpointFilter<FluentValidationEndpointFilter>();
```

You can also add the filter to a group instead, which will apply it to all of its endpoints, like this:

```csharp
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

1. You can tell the filter to scan all parameters (`ScanAllParams`), which enables you to not worry about parameter order since the filter will always try to validate all of them when it finds an `IValidator<T>` instance that matches its type. **This is the default behavior**.
1. You can tell the filter to scan parameters until there is no validator found (`ScanUntilNoValidatorFound`). This forces you to add your objects to the lefthand side of the delegates. This is more optimal but comes with one constraint.
