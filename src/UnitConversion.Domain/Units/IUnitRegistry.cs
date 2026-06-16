namespace UnitConversion.Domain.Units;

/// <summary>
/// Source-of-truth lookup for categories and their units.
///
/// The interface deliberately hides the storage mechanism so the seeded,
/// hardcoded implementation used today can be swapped for a database- or
/// configuration-backed implementation later without changing callers.
/// </summary>
public interface IUnitRegistry
{
    IReadOnlyCollection<UnitCategory> GetCategories();

    bool TryGetCategory(string categoryName, out UnitCategory category);

    IReadOnlyCollection<Unit> GetUnits(string categoryName);

    /// <summary>
    /// Resolve a unit by symbol, name, or any registered alias (case-insensitive).
    /// </summary>
    bool TryResolveUnit(string identifier, out Unit unit);
}
