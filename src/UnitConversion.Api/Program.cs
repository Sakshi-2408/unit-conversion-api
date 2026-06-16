using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UnitConversion.Api.Infrastructure.ExceptionHandling;
using UnitConversion.Api.Infrastructure.Swagger;
using UnitConversion.Application;

// Bootstrap logger so startup failures are recorded too.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services
        .AddUnitConversion()
        .AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            // Ensure model-binding errors are returned as ProblemDetails too.
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://httpstatuses.io/400",
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path,
                };

                return new BadRequestObjectResult(details)
                {
                    ContentTypes = { "application/problem+json" },
                };
            };
        });

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

    builder.Services
        .AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    // Swagger is exposed in every environment so reviewers and integration
    // consumers can discover and exercise the API without configuring anything.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.Services
            .GetRequiredService<IApiVersionDescriptionProvider>()
            .ApiVersionDescriptions;

        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Unit Conversion API {description.GroupName.ToUpperInvariant()}");
        }

        options.DocumentTitle = "Unit Conversion API - Swagger UI";
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.DefaultModelsExpandDepth(-1);
        options.EnableTryItOutByDefault();
    });

    // One-click landing experience: hitting the root sends you to Swagger UI.
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Exposed for WebApplicationFactory in integration tests (future work).
public partial class Program;
