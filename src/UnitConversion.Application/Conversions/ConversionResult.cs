namespace UnitConversion.Application.Conversions;

/// <summary>
/// Outcome of a successful conversion.
/// </summary>
/// <param name="Value">Converted value, expressed in <paramref name="ToUnit"/>.</param>
/// <param name="OriginalValue">Echo of the originally supplied value.</param>
/// <param name="FromUnit">Canonical symbol of the source unit.</param>
/// <param name="ToUnit">Canonical symbol of the target unit.</param>
/// <param name="Category">Category both units belong to.</param>
public sealed record ConversionResult(
    double Value,
    double OriginalValue,
    string FromUnit,
    string ToUnit,
    string Category);
