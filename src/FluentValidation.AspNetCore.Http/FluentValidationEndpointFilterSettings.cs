namespace FluentValidation.AspNetCore.Http;

/// <summary>
/// Contains the settings that configure the behaviors of the <see cref="FluentValidationEndpointFilter"/> class.
/// </summary>
public class FluentValidationEndpointFilterSettings
{
    /// <summary>
    /// The dependency scanning strategy.<br /><br />
    /// <strong>Default:</strong> <see cref="ScanningStrategy.ScanAllParams"/>.
    /// </summary>
    public required ScanningStrategy ScanningStrategy { get; set; } = ScanningStrategy.ScanAllParams;
}

/// <summary>
/// The scanning strategy used by the <see cref="FluentValidationEndpointFilter"/> class.
/// </summary>
public enum ScanningStrategy
{
    /// <summary>
    /// The filter scans all parameters.
    /// For each parameter, it tries to get an <see cref="IValidator&lt;T&rt;"/> instance from the ASP.NET Core container.
    /// When it finds one, the filter validates the parameter.
    /// </summary>
    ScanAllParams,

    /// <summary>
    /// The filter scans parameters until it does not find a validator for a parameter. When that happens, the validation stops.
    /// </summary>
    /// <remarks>
    /// This can be a more optimal scanning strategy, but the parameters to validate must be the first ones of the endpoint delegate.
    /// </remarks>
    ScanUntilNoValidatorFound
}