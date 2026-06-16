using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UnitConversion.Api.Contracts;
using UnitConversion.Application.Conversions;

namespace UnitConversion.Api.Controllers.V1;

/// <summary>
/// Convert numerical values between units. Source and target units must belong
/// to the same category (e.g. you cannot convert <c>kg</c> to <c>m</c>).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/conversions")]
[Produces("application/json")]
[SwaggerTag("Convert numerical values between units of the same category.")]
public sealed class ConversionsController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IValidator<ConversionRequest> _validator;

    public ConversionsController(
        IConversionService conversionService,
        IValidator<ConversionRequest> validator)
    {
        _conversionService = conversionService;
        _validator = validator;
    }

    /// <summary>Convert a value between two units (JSON body).</summary>
    /// <remarks>
    /// Submit the value together with the source and target units. Unit
    /// identifiers may be a canonical symbol (<c>km</c>, <c>°C</c>), the full
    /// name (<c>kilometer</c>), or any registered alias (<c>kilometres</c>).
    ///
    /// Example request body:
    /// ```json
    /// {
    ///   "value": 100,
    ///   "fromUnit": "km",
    ///   "toUnit": "mi"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">The conversion request.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">The conversion succeeded.</response>
    /// <response code="400">The request was malformed or the units were incompatible.</response>
    /// <response code="404">One or both unit identifiers were not recognised.</response>
    [HttpPost(Name = "ConvertValue")]
    [SwaggerOperation(Summary = "Convert a value (POST)", OperationId = "ConvertValue")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversionResponse>> Convert(
        [FromBody] ConversionRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = _conversionService.Convert(request);
        return Ok(Map(result));
    }

    /// <summary>Convert a value between two units (query string).</summary>
    /// <remarks>
    /// Convenience GET form of the conversion endpoint, suitable for
    /// exploration in a browser or a quick <c>curl</c> call.
    ///
    /// Example: <c>GET /api/v1/conversions?value=100&amp;from=C&amp;to=F</c>
    /// </remarks>
    /// <param name="value">Numerical value to convert (must be finite).</param>
    /// <param name="from">Source unit identifier (symbol, name, or alias).</param>
    /// <param name="to">Target unit identifier (symbol, name, or alias).</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="200">The conversion succeeded.</response>
    /// <response code="400">The query string was invalid or the units were incompatible.</response>
    /// <response code="404">One or both unit identifiers were not recognised.</response>
    [HttpGet(Name = "ConvertValueByQuery")]
    [SwaggerOperation(Summary = "Convert a value (GET)", OperationId = "ConvertValueByQuery")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversionResponse>> ConvertByQuery(
        [FromQuery, SwaggerParameter("Numerical value to convert.", Required = true)] double value,
        [FromQuery, SwaggerParameter("Source unit identifier (symbol, name, or alias).", Required = true)] string from,
        [FromQuery, SwaggerParameter("Target unit identifier (symbol, name, or alias).", Required = true)] string to,
        CancellationToken cancellationToken)
    {
        var request = new ConversionRequest(value, from, to);
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = _conversionService.Convert(request);
        return Ok(Map(result));
    }

    private static ConversionResponse Map(ConversionResult result) =>
        new(result.Value, result.OriginalValue, result.FromUnit, result.ToUnit, result.Category);
}
