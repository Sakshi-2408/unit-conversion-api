using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Conversions;
using UnitConversion.Application.Units;
using UnitConversion.Application.Units.Seeds;
using UnitConversion.Domain.Units;

namespace UnitConversion.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the application services: seeds, registry, conversion service,
    /// and validators. Consumers (the API project, future workers, tests, ...)
    /// call this single method.
    /// </summary>
    public static IServiceCollection AddUnitConversion(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IUnitSeed, LengthUnitSeed>();
        services.AddSingleton<IUnitSeed, MassUnitSeed>();
        services.AddSingleton<IUnitSeed, TemperatureUnitSeed>();

        services.AddSingleton<IUnitRegistry, SeededUnitRegistry>();
        services.AddSingleton<IConversionService, ConversionService>();

        services.AddValidatorsFromAssemblyContaining<ConversionRequestValidator>();

        return services;
    }
}
