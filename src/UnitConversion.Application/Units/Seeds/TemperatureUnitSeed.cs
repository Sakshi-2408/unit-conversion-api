using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units.Seeds;

/// <summary>
/// Temperature units. Canonical base unit: <c>kelvin</c>.
///
/// Temperature scales are affine (offsets, not just multiplicative factors),
/// so we provide explicit delegate pairs rather than the linear shortcut.
/// </summary>
public sealed class TemperatureUnitSeed : IUnitSeed
{
    private const string CategoryName = "Temperature";

    public UnitCategory Category { get; } = new(CategoryName, baseUnitSymbol: "K");

    public IReadOnlyCollection<Unit> Units { get; } = new[]
    {
        new Unit(
            symbol: "K",
            name: "kelvin",
            categoryName: CategoryName,
            toBase: k => k,
            fromBase: k => k,
            aliases: new[] { "kelvins" }),

        new Unit(
            symbol: "C",
            name: "celsius",
            categoryName: CategoryName,
            toBase: c => c + 273.15,
            fromBase: k => k - 273.15,
            aliases: new[] { "°C", "degC", "centigrade" }),

        new Unit(
            symbol: "F",
            name: "fahrenheit",
            categoryName: CategoryName,
            toBase: f => (f - 32.0) * 5.0 / 9.0 + 273.15,
            fromBase: k => (k - 273.15) * 9.0 / 5.0 + 32.0,
            aliases: new[] { "°F", "degF" }),

        new Unit(
            symbol: "R",
            name: "rankine",
            categoryName: CategoryName,
            toBase: r => r * 5.0 / 9.0,
            fromBase: k => k * 9.0 / 5.0,
            aliases: new[] { "°R", "degR" }),
    };
}
