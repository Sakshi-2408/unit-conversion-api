using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UnitConversion.Api.Infrastructure.Swagger;

/// <summary>
/// Registers a Swagger document per discovered API version and wires up XML
/// documentation from every relevant assembly so that summaries, parameter
/// descriptions and response notes show up in the UI.
/// </summary>
public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, BuildInfo(description));
        }

        IncludeXmlComments(options);

        options.EnableAnnotations();
        options.SupportNonNullableReferenceTypes();
        options.DescribeAllParametersInCamelCase();
    }

    private static OpenApiInfo BuildInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "Unit Conversion API",
            Version = description.ApiVersion.ToString(),
            Description = """
                A small, extensible HTTP API for converting numerical values between
                units of measurement (length, mass, temperature, ...).

                The endpoints are versioned (`/api/v{version}/...`) and return RFC 7807
                `application/problem+json` bodies on error.
                """,
            License = new OpenApiLicense { Name = "MIT" },
        };

        if (description.IsDeprecated)
        {
            info.Description += " **This API version has been deprecated.**";
        }

        return info;
    }

    private static void IncludeXmlComments(SwaggerGenOptions options)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var assemblies = new[]
        {
            typeof(Program).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Domain.Units.Unit).Assembly,
        };

        foreach (var assembly in assemblies)
        {
            var xmlPath = Path.Combine(baseDirectory, $"{assembly.GetName().Name}.xml");
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        }

        // Quiet the analyzer about Assembly being unused if nothing matches.
        _ = typeof(Assembly);
    }
}
