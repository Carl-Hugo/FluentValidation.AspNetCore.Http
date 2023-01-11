namespace FluentValidation.AspNetCore.Http;

/// <summary>
/// Contains the settings that configure the behaviors of the <see cref="FluentValidationEndpointFilter"/> class.
/// </summary>
public class FluentValidationEndpointFilterSettings
{
    /// <summary>
    /// The dependency scanning stragtegy.<br /><br />
    /// <strong>Default:</strong> <see cref="ScanningStrategy.ScanUntilNoValidatorFound"/>.
    /// </summary>
    public required ScanningStrategy ScanningStrategy { get; set; } = ScanningStrategy.ScanUntilNoValidatorFound;
}

/// <summary>
/// The scanning strategy used by the <see cref="FluentValidationEndpointFilter"/> class.
/// </summary>
public enum ScanningStrategy
{
    /// <summary>
    /// The validation will be done for all Arguments.
    /// </summary>
    ScanAllParams,

    /// <summary>
    /// The validation will stop scanning for Arguments after the first argument is found without a validator.
    /// </summary>
    /// <remarks>
    /// This is an optimal scanning strategy, but it requires to put the parameters to validate first.
    /// </remarks>
    ScanUntilNoValidatorFound
}