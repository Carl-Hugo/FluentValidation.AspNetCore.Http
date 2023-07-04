using FluentValidation.AspNetCore.Http;
using FluentValidation.AspNetCore.Http.ResultsFactory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

public static class FluentValidationEndpointFilterExtensions
{
    /// <summary>
    /// Registers FluentValidationEndpointFilter dependencies with the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to registers dependencies with.</param>
    /// <returns>The provided <see cref="WebApplicationBuilder"/> instance.</returns>
    public static WebApplicationBuilder AddFluentValidationEndpointFilter(this WebApplicationBuilder builder, Action<FluentValidationEndpointFilterSettings>? configureOptions = null)
    {
        builder.Services
            .AddOptions<FluentValidationEndpointFilterSettings>()
            .Bind(builder.Configuration.GetSection("FluentValidationEndpointFilter"))
            .Configure(settings => configureOptions?.Invoke(settings))
        ;
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<FluentValidationEndpointFilterSettings>>().Value);
        builder.Services.TryAddSingleton<IFluentValidationEndpointFilterResultsFactory, SimpleResultsFactory>();
        return builder;
    }

    /// <summary>
    /// Registers a filter of type <typeparamref name="FluentValidationEndpointFilter"/> onto the route handler.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the route handler.</returns>
    public static RouteHandlerBuilder AddFluentValidationFilter(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter<FluentValidationEndpointFilter>();
        return builder;
    }

    /// <summary>
    /// Registers a filter of type <typeparamref name="FluentValidationEndpointFilter"/> onto the route handler.
    /// </summary>
    /// <param name="builder">The <see cref="RouteGroupBuilder"/>.</param>
    /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route handler.</returns>
    public static RouteGroupBuilder AddFluentValidationFilter(this RouteGroupBuilder builder)
    {
        builder.AddEndpointFilter<FluentValidationEndpointFilter>();
        return builder;
    }
}
