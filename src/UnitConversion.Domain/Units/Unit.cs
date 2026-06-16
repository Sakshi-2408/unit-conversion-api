namespace UnitConversion.Domain.Units;

/// <summary>
/// A single unit of measurement (e.g. <c>kilometer</c>, <c>fahrenheit</c>).
///
/// Conversion is expressed via two delegates that move a value to and from the
/// category's canonical base unit. For linear units this is equivalent to a
/// multiplicative factor, but the delegate form also accommodates affine
/// conversions (e.g. Celsius &lt;-&gt; Kelvin) without special-casing them later.
/// </summary>
public sealed class Unit
{
    private readonly Func<double, double> _toBase;
    private readonly Func<double, double> _fromBase;

    public Unit(
        string symbol,
        string name,
        string categoryName,
        Func<double, double> toBase,
        Func<double, double> fromBase,
        IReadOnlyCollection<string>? aliases = null)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol is required.", nameof(symbol));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required.", nameof(categoryName));
        }

        Symbol = symbol;
        Name = name;
        CategoryName = categoryName;
        _toBase = toBase ?? throw new ArgumentNullException(nameof(toBase));
        _fromBase = fromBase ?? throw new ArgumentNullException(nameof(fromBase));
        Aliases = aliases ?? Array.Empty<string>();
    }

    /// <summary>Canonical symbol (e.g. <c>km</c>, <c>°C</c>).</summary>
    public string Symbol { get; }

    /// <summary>Human friendly name (e.g. <c>kilometer</c>).</summary>
    public string Name { get; }

    /// <summary>Name of the category this unit belongs to.</summary>
    public string CategoryName { get; }

    /// <summary>Optional alternative identifiers callers may pass.</summary>
    public IReadOnlyCollection<string> Aliases { get; }

    /// <summary>Convert a value expressed in this unit to the category base unit.</summary>
    public double ToBase(double value) => _toBase(value);

    /// <summary>Convert a value expressed in the category base unit to this unit.</summary>
    public double FromBase(double baseValue) => _fromBase(baseValue);

    /// <summary>
    /// Convenience factory for purely linear conversions, where
    /// <c>baseValue = value * factor</c>.
    /// </summary>
    public static Unit Linear(
        string symbol,
        string name,
        string categoryName,
        double factorToBase,
        IReadOnlyCollection<string>? aliases = null)
    {
        if (factorToBase <= 0 || double.IsNaN(factorToBase) || double.IsInfinity(factorToBase))
        {
            throw new ArgumentException(
                "Factor must be a positive finite number.",
                nameof(factorToBase));
        }

        return new Unit(
            symbol,
            name,
            categoryName,
            value => value * factorToBase,
            baseValue => baseValue / factorToBase,
            aliases);
    }
}
