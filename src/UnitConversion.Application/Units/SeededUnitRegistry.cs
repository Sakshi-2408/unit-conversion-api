using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units;

/// <summary>
/// In-memory <see cref="IUnitRegistry"/> backed by hardcoded seed data.
///
/// This is the only place that knows what the supported categories and units
/// are. Adding a unit is a one-line change; swapping the data source (e.g. to
/// a database or configuration file) means writing a new implementation of
/// <see cref="IUnitRegistry"/> rather than editing callers.
/// </summary>
public sealed class SeededUnitRegistry : IUnitRegistry
{
    private readonly IReadOnlyDictionary<string, UnitCategory> _categoriesByName;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<Unit>> _unitsByCategory;
    private readonly IReadOnlyDictionary<string, Unit> _unitsByIdentifier;

    public SeededUnitRegistry(IEnumerable<IUnitSeed> seeds)
    {
        ArgumentNullException.ThrowIfNull(seeds);

        var categories = new Dictionary<string, UnitCategory>(StringComparer.OrdinalIgnoreCase);
        var unitsByCategory = new Dictionary<string, List<Unit>>(StringComparer.OrdinalIgnoreCase);
        var unitsByIdentifier = new Dictionary<string, Unit>(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in seeds)
        {
            var category = seed.Category;
            if (!categories.TryAdd(category.Name, category))
            {
                throw new InvalidOperationException(
                    $"Duplicate category registration for '{category.Name}'.");
            }

            var units = seed.Units.ToList();
            unitsByCategory[category.Name] = units;

            foreach (var unit in units)
            {
                RegisterIdentifier(unitsByIdentifier, unit.Symbol, unit);
                RegisterIdentifier(unitsByIdentifier, unit.Name, unit);
                foreach (var alias in unit.Aliases)
                {
                    RegisterIdentifier(unitsByIdentifier, alias, unit);
                }
            }
        }

        _categoriesByName = categories;
        _unitsByCategory = unitsByCategory.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<Unit>)kvp.Value,
            StringComparer.OrdinalIgnoreCase);
        _unitsByIdentifier = unitsByIdentifier;
    }

    public IReadOnlyCollection<UnitCategory> GetCategories() => _categoriesByName.Values.ToArray();

    public bool TryGetCategory(string categoryName, out UnitCategory category)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            category = null!;
            return false;
        }

        return _categoriesByName.TryGetValue(categoryName, out category!);
    }

    public IReadOnlyCollection<Unit> GetUnits(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)
            || !_unitsByCategory.TryGetValue(categoryName, out var units))
        {
            return Array.Empty<Unit>();
        }

        return units;
    }

    public bool TryResolveUnit(string identifier, out Unit unit)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            unit = null!;
            return false;
        }

        return _unitsByIdentifier.TryGetValue(identifier.Trim(), out unit!);
    }

    private static void RegisterIdentifier(
        IDictionary<string, Unit> map,
        string identifier,
        Unit unit)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return;
        }

        if (map.TryGetValue(identifier, out var existing) && !ReferenceEquals(existing, unit))
        {
            throw new InvalidOperationException(
                $"Identifier '{identifier}' is already registered to unit '{existing.Symbol}'.");
        }

        map[identifier] = unit;
    }
}
