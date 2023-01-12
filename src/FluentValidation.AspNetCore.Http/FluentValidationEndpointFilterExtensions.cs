using FluentValidation.AspNetCore.Http;
using FluentValidation.AspNetCore.Http.ResultsFactory;
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
}
