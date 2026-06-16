using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UnitConversion.Api.Contracts;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Units;

namespace UnitConversion.Api.Controllers.V1;

/// <summary>
/// Discovery endpoints for the categories of measurement supported by the API
/// (for example <c>Length</c>, <c>Mass</c>, <c>Temperature</c>) and the units
/// inside each one.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
[Produces("application/json")]
[SwaggerTag("Browse the supported categories and the units they contain.")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IUnitRegistry _registry;

    public CategoriesController(IUnitRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>List every supported conversion category.</summary>
    /// <remarks>
    /// Use this to discover the categories the API currently knows about
    /// (e.g. <c>Length</c>, <c>Mass</c>, <c>Temperature</c>). Each category
    /// reports the symbol of the canonical base unit used internally for
    /// conversions.
    /// </remarks>
    /// <response code="200">A list of categories.</response>
    [HttpGet(Name = "ListCategories")]
    [SwaggerOperation(Summary = "List supported categories", OperationId = "ListCategories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CategoryResponse>> List()
    {
        var response = _registry
            .GetCategories()
            .Select(c => new CategoryResponse(c.Name, c.BaseUnitSymbol))
            .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);

        return Ok(response);
    }

    /// <summary>List all units belonging to a single category.</summary>
    /// <remarks>
    /// The <paramref name="category"/> name is matched case-insensitively.
    /// Each unit reports its canonical symbol (the value you should pass to
    /// the conversion endpoint), its human-readable name, and any aliases
    /// the API will also accept.
    /// </remarks>
    /// <param name="category">Category name, e.g. <c>Length</c>.</param>
    /// <response code="200">The list of units in the category.</response>
    /// <response code="404">No category exists with that name.</response>
    [HttpGet("{category}/units", Name = "ListUnitsForCategory")]
    [SwaggerOperation(Summary = "List units for a category", OperationId = "ListUnitsForCategory")]
    [ProducesResponseType(typeof(IEnumerable<UnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<UnitResponse>> GetUnits(
        [SwaggerParameter("Category name (case-insensitive), e.g. 'Length'.")] string category)
    {
        if (!_registry.TryGetCategory(category, out var resolved))
        {
            throw new CategoryNotFoundException(category);
        }

        var response = _registry
            .GetUnits(resolved.Name)
            .Select(u => new UnitResponse(u.Symbol, u.Name, u.CategoryName, u.Aliases))
            .OrderBy(u => u.Symbol, StringComparer.OrdinalIgnoreCase);

        return Ok(response);
    }
}
