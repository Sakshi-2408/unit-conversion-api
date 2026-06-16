using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units.Seeds;

/// <summary>
/// Length units. Canonical base unit: <c>meter</c>.
/// Factors are the number of meters in one of the given unit.
/// </summary>
public sealed class LengthUnitSeed : IUnitSeed
{
    private const string CategoryName = "Length";

    public UnitCategory Category { get; } = new(CategoryName, baseUnitSymbol: "m");

    public IReadOnlyCollection<Unit> Units { get; } = new[]
    {
        Unit.Linear("mm",  "millimeter", CategoryName, 0.001,    new[] { "millimeters", "millimetre", "millimetres" }),
        Unit.Linear("cm",  "centimeter", CategoryName, 0.01,     new[] { "centimeters", "centimetre", "centimetres" }),
        Unit.Linear("m",   "meter",      CategoryName, 1.0,      new[] { "meters", "metre", "metres" }),
        Unit.Linear("km",  "kilometer",  CategoryName, 1000.0,   new[] { "kilometers", "kilometre", "kilometres" }),
        Unit.Linear("in",  "inch",       CategoryName, 0.0254,   new[] { "inches" }),
        Unit.Linear("ft",  "foot",       CategoryName, 0.3048,   new[] { "feet" }),
        Unit.Linear("yd",  "yard",       CategoryName, 0.9144,   new[] { "yards" }),
        Unit.Linear("mi",  "mile",       CategoryName, 1609.344, new[] { "miles" }),
        Unit.Linear("nmi", "nautical-mile", CategoryName, 1852.0, new[] { "nautical-miles", "nautical_mile" }),
    };
}
