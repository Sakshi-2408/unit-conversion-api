namespace UnitConversion.Domain.Exceptions;

public sealed class UnitNotFoundException : Exception
{
    public UnitNotFoundException(string identifier)
        : base($"No unit was found matching identifier '{identifier}'.")
    {
        Identifier = identifier;
    }

    public string Identifier { get; }
}
