using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units.Seeds;

/// <summary>
/// Mass / weight units. Canonical base unit: <c>kilogram</c>.
/// Factors are the number of kilograms in one of the given unit.
/// </summary>
public sealed class MassUnitSeed : IUnitSeed
{
    private const string CategoryName = "Mass";

    public UnitCategory Category { get; } = new(CategoryName, baseUnitSymbol: "kg");

    public IReadOnlyCollection<Unit> Units { get; } = new[]
    {
        Unit.Linear("mg", "milligram",  CategoryName, 0.000_001, new[] { "milligrams" }),
        Unit.Linear("g",  "gram",       CategoryName, 0.001,     new[] { "grams" }),
        Unit.Linear("kg", "kilogram",   CategoryName, 1.0,       new[] { "kilograms" }),
        Unit.Linear("t",  "metric-ton", CategoryName, 1_000.0,   new[] { "tonne", "tonnes", "metric-tons" }),
        Unit.Linear("oz", "ounce",      CategoryName, 0.028_349_523_125, new[] { "ounces" }),
        Unit.Linear("lb", "pound",      CategoryName, 0.453_592_37,      new[] { "lbs", "pounds" }),
        Unit.Linear("st", "stone",      CategoryName, 6.350_293_18,      new[] { "stones" }),
    };
}
