namespace UnitConversion.Api.Contracts;

/// <summary>A category of measurement supported by the API.</summary>
/// <param name="Name">Display name of the category, e.g. <c>Length</c>.</param>
/// <param name="BaseUnit">Symbol of the canonical base unit (e.g. <c>m</c>, <c>kg</c>, <c>K</c>).</param>
public sealed record CategoryResponse(string Name, string BaseUnit);
