using Microsoft.AspNetCore.Builder;

namespace FluentValidation.AspNetCore.Http.Tests;

public class FluentValidationEndpointFilterTestApp : MinimalHostingTestApp
{
    public FluentValidationEndpointFilterTestApp(
        Action<WebApplicationBuilder>? configureBuilder = default,
        Action<WebApplication>? configureApp = default,
        Action<FluentValidationEndpointFilterSettings>? configureFilter = default)
        : base((builder) =>
        {
            builder.AddFluentValidationEndpointFilter(configureFilter);
            configureBuilder?.Invoke(builder);
        }, configureApp)
    {
    }
}
