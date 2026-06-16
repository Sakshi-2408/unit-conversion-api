namespace UnitConversion.Application.Conversions;

/// <summary>
/// Inbound payload for a conversion request.
/// </summary>
/// <param name="Value">The numerical value to convert.</param>
/// <param name="FromUnit">Source unit identifier (symbol, name, or alias).</param>
/// <param name="ToUnit">Target unit identifier (symbol, name, or alias).</param>
public sealed record ConversionRequest(double Value, string FromUnit, string ToUnit);
