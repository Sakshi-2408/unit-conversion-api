using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Domain.Exceptions;

namespace UnitConversion.Api.Infrastructure.ExceptionHandling;

/// <summary>
/// Translates exceptions thrown deeper in the stack into RFC 7807
/// <see cref="ProblemDetails"/> responses with appropriate status codes.
/// </summary>
public sealed class ProblemDetailsExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ProblemDetailsExceptionHandler> _logger;

    public ProblemDetailsExceptionHandler(ILogger<ProblemDetailsExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            ValidationException ve => new ProblemDetails
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)),
                Type = "https://httpstatuses.io/400",
            },
            UnitNotFoundException nf => new ProblemDetails
            {
                Title = "Unit not found",
                Status = StatusCodes.Status404NotFound,
                Detail = nf.Message,
                Type = "https://httpstatuses.io/404",
            },
            CategoryNotFoundException cnf => new ProblemDetails
            {
                Title = "Category not found",
                Status = StatusCodes.Status404NotFound,
                Detail = cnf.Message,
                Type = "https://httpstatuses.io/404",
            },
            IncompatibleUnitsException iu => new ProblemDetails
            {
                Title = "Incompatible units",
                Status = StatusCodes.Status400BadRequest,
                Detail = iu.Message,
                Type = "https://httpstatuses.io/400",
            },
            _ => null,
        };

        if (problem is null)
        {
            _logger.LogError(exception, "Unhandled exception");
            return false;
        }

        problem.Instance = httpContext.Request.Path;
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
