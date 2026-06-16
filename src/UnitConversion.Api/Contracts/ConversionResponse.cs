namespace UnitConversion.Api.Contracts;

/// <summary>Successful conversion result.</summary>
/// <param name="Value">The converted value, expressed in <paramref name="ToUnit"/>.</param>
/// <param name="OriginalValue">Echo of the value supplied in the request.</param>
/// <param name="FromUnit">Canonical symbol of the source unit that was resolved.</param>
/// <param name="ToUnit">Canonical symbol of the target unit that was resolved.</param>
/// <param name="Category">Category both units belong to.</param>
public sealed record ConversionResponse(
    double Value,
    double OriginalValue,
    string FromUnit,
    string ToUnit,
    string Category);
