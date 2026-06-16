using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units;

/// <summary>
/// Contributes a single category and its units to the registry.
/// Implementations are registered in DI and picked up automatically.
/// </summary>
public interface IUnitSeed
{
    UnitCategory Category { get; }

    IReadOnlyCollection<Unit> Units { get; }
}
