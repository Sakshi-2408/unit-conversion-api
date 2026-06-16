namespace UnitConversion.Domain.Exceptions;

public sealed class IncompatibleUnitsException : Exception
{
    public IncompatibleUnitsException(string fromCategory, string toCategory)
        : base($"Cannot convert between units from different categories ('{fromCategory}' and '{toCategory}').")
    {
        FromCategory = fromCategory;
        ToCategory = toCategory;
    }

    public string FromCategory { get; }

    public string ToCategory { get; }
}
