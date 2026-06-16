using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Conversions;

public sealed class ConversionService : IConversionService
{
    private readonly IUnitRegistry _registry;

    public ConversionService(IUnitRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public ConversionResult Convert(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!_registry.TryResolveUnit(request.FromUnit, out var fromUnit))
        {
            throw new UnitNotFoundException(request.FromUnit);
        }

        if (!_registry.TryResolveUnit(request.ToUnit, out var toUnit))
        {
            throw new UnitNotFoundException(request.ToUnit);
        }

        if (!string.Equals(fromUnit.CategoryName, toUnit.CategoryName, StringComparison.OrdinalIgnoreCase))
        {
            throw new IncompatibleUnitsException(fromUnit.CategoryName, toUnit.CategoryName);
        }

        // Two-step conversion via the category's base unit keeps the math
        // uniform across linear (length, weight) and affine (temperature) units.
        var baseValue = fromUnit.ToBase(request.Value);
        var converted = toUnit.FromBase(baseValue);

        return new ConversionResult(
            Value: converted,
            OriginalValue: request.Value,
            FromUnit: fromUnit.Symbol,
            ToUnit: toUnit.Symbol,
            Category: fromUnit.CategoryName);
    }
}
