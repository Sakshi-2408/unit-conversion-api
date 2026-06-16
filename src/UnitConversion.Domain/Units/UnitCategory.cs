namespace UnitConversion.Domain.Units;

/// <summary>
/// A category groups together units that are convertible to one another
/// (e.g. all <c>Length</c> units share <c>meter</c> as the canonical base).
/// </summary>
public sealed class UnitCategory
{
    public UnitCategory(string name, string baseUnitSymbol)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(baseUnitSymbol))
        {
            throw new ArgumentException("Base unit symbol is required.", nameof(baseUnitSymbol));
        }

        Name = name;
        BaseUnitSymbol = baseUnitSymbol;
    }

    public string Name { get; }

    public string BaseUnitSymbol { get; }
}
