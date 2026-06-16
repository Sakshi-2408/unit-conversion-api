namespace UnitConversion.Api.Contracts;

/// <summary>A single unit of measurement exposed by the API.</summary>
/// <param name="Symbol">Canonical symbol of the unit, e.g. <c>km</c>.</param>
/// <param name="Name">Human friendly name, e.g. <c>kilometer</c>.</param>
/// <param name="Category">Category the unit belongs to, e.g. <c>Length</c>.</param>
/// <param name="Aliases">Alternative identifiers callers may pass as <c>fromUnit</c> / <c>toUnit</c>.</param>
public sealed record UnitResponse(
    string Symbol,
    string Name,
    string Category,
    IReadOnlyCollection<string> Aliases);
