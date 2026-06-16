namespace UnitConversion.Application.Conversions;

public interface IConversionService
{
    ConversionResult Convert(ConversionRequest request);
}
