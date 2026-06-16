namespace UnitConversion.Domain.Exceptions;

public sealed class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(string categoryName)
        : base($"No category was found with name '{categoryName}'.")
    {
        CategoryName = categoryName;
    }

    public string CategoryName { get; }
}
