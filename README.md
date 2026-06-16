# Unit Conversion API

A small, extensible ASP.NET Core Web API that converts numerical values between
units of measurement (length, mass, temperature). The codebase is laid out as a
real-world team project would be: layered architecture, clear separation of
concerns, dependency injection, validation, structured logging, versioned
routes, and full OpenAPI/Swagger documentation.

---

## Table of contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Run it locally](#run-it-locally)
4. [Try the API](#try-the-api)
5. [Project structure](#project-structure)
6. [Supported categories and units](#supported-categories-and-units)
7. [Adding a new unit or category](#adding-a-new-unit-or-category)
8. [Design decisions and trade-offs](#design-decisions-and-trade-offs)
9. [Tech stack](#tech-stack)

---

## Overview

The API exposes a small surface:

| Method | Route                                       | Purpose                                   |
|-------:|---------------------------------------------|-------------------------------------------|
| `GET`  | `/api/v1/categories`                        | List all supported categories             |
| `GET`  | `/api/v1/categories/{category}/units`       | List the units inside a category          |
| `POST` | `/api/v1/conversions`                       | Convert a value (JSON body)               |
| `GET`  | `/api/v1/conversions?value=&from=&to=`      | Convert a value (query string convenience)|

All responses are JSON. Errors are returned as RFC 7807
`application/problem+json` bodies.

---

## Prerequisites

- **.NET SDK 8.0** or newer
  ([download](https://dotnet.microsoft.com/download))
- A REST client of your choice for trying calls: Swagger UI (built in), `curl`,
  Postman, the bundled `*.http` file in JetBrains Rider / Visual Studio, etc.

Check your installation:

```bash
dotnet --version
# 8.0.x or higher
```

---

## Run it locally

```bash
# 1. Restore and build
dotnet build UnitConversion.sln

# 2. Run the API
dotnet run --project src/UnitConversion.Api
```

The API will start on the URL printed in the console (by default
`http://localhost:5012`). To pin the URL explicitly:

```bash
dotnet run --project src/UnitConversion.Api --urls http://localhost:5012
```

Once it's up:

- **Swagger UI** (recommended for exploration): <http://localhost:5012/swagger>
- **OpenAPI document**: <http://localhost:5012/swagger/v1/swagger.json>
- The root URL `/` redirects to Swagger UI

Stop the server with `Ctrl+C`.

---

## Try the API

### Via Swagger UI (recommended walkthrough)

This is the fastest way to verify every endpoint without leaving the browser.
Every operation in Swagger is annotated with a summary, parameter
descriptions, and the list of possible status codes.

#### Open the UI

1. Start the API (`dotnet run --project src/UnitConversion.Api`).
2. Browse to <http://localhost:5012/swagger> (or hit
   <http://localhost:5012/> — the root redirects there).
3. You should see two tag groups: **Categories** and **Conversions**, each
   expanded to show its operations.

#### How to use "Try it out"

For every endpoint the workflow is the same:

1. Click the operation row to expand it.
2. Click the **Try it out** button on the right.
3. Fill in the input fields (parameters or request body).
4. Click **Execute**.
5. Inspect the **Response body** and **Response code** sections below the
   button. The exact `curl` command Swagger sent is also shown — useful for
   copy-pasting into a terminal.

#### Suggested test scenarios

Run through these in order. Each one exercises a different aspect of the API.

| # | Endpoint | Inputs | Expected status | Expected outcome |
|---|----------|--------|-----------------|------------------|
| 1 | `GET /api/v1/categories` | _(none)_ | **200** | 3 items: `Length` / `m`, `Mass` / `kg`, `Temperature` / `K` |
| 2 | `GET /api/v1/categories/{category}/units` | `category = Length` | **200** | 9 length units with their aliases |
| 3 | `GET /api/v1/categories/{category}/units` | `category = Temperature` | **200** | 4 units: K, C, F, R |
| 4 | `GET /api/v1/conversions` | `value=100`, `from=km`, `to=mi` | **200** | `value ≈ 62.137` |
| 5 | `GET /api/v1/conversions` | `value=100`, `from=C`, `to=F` | **200** | `value = 212` |
| 6 | `GET /api/v1/conversions` | `value=0`, `from=C`, `to=K` | **200** | `value = 273.15` |
| 7 | `GET /api/v1/conversions` | `value=1`, `from=kg`, `to=lb` | **200** | `value ≈ 2.2046` |
| 8 | `GET /api/v1/conversions` | `value=12`, `from=inches`, `to=ft` | **200** | `value ≈ 1` (proves **alias resolution** works) |
| 9 | `POST /api/v1/conversions` | body: `{"value":100,"fromUnit":"kilometers","toUnit":"miles"}` | **200** | Same as #4 — confirms POST + aliases |

#### Error cases worth verifying

These return RFC 7807 `ProblemDetails` bodies — confirming that the API fails
gracefully and that clients get a consistent error contract.

| # | Endpoint | Inputs | Expected status | Why |
|---|----------|--------|-----------------|-----|
| E1 | `GET /api/v1/conversions` | `value=10`, `from=km`, `to=kg` | **400** `Incompatible units` | Length and Mass can't be cross-converted |
| E2 | `GET /api/v1/conversions` | `value=10`, `from=furlong`, `to=mi` | **404** `Unit not found` | `furlong` isn't registered |
| E3 | `GET /api/v1/categories/{category}/units` | `category=Pressure` | **404** `Category not found` | Only Length/Mass/Temperature are seeded |
| E4 | `POST /api/v1/conversions` | body: `{"value":1,"fromUnit":"","toUnit":"m"}` | **400** `Validation failed` | FluentValidation rejects empty `fromUnit` |

#### Quick checklist for the reviewer

- [ ] Swagger UI loads at `/swagger` without auth or extra config
- [ ] Every operation shows a description and parameter docs
- [ ] All 9 happy-path scenarios above return **200** with the expected value
- [ ] All 4 error cases return the listed **400 / 404** with a
      `ProblemDetails` body (`type`, `title`, `status`, `detail`, `instance`)
- [ ] Aliases (`kilometers`, `miles`, `inches`, `°C`, etc.) resolve correctly

### Via `curl`

```bash
# List categories
curl http://localhost:5012/api/v1/categories

# List length units
curl http://localhost:5012/api/v1/categories/Length/units

# Convert 100 km to miles (query-string form)
curl "http://localhost:5012/api/v1/conversions?value=100&from=km&to=mi"

# Convert 100 C to F (POST body form)
curl -X POST http://localhost:5012/api/v1/conversions \
  -H "Content-Type: application/json" \
  -d '{"value":100,"fromUnit":"C","toUnit":"F"}'
```

### Via the bundled `.http` file

Open `src/UnitConversion.Api/UnitConversion.Api.http` in Visual Studio,
JetBrains Rider, or VS Code (with the REST Client extension) and click the
request you want to send.

### Example responses

Success:

```json
{
  "value": 62.13711922373339,
  "originalValue": 100,
  "fromUnit": "km",
  "toUnit": "mi",
  "category": "Length"
}
```

Error (incompatible units):

```json
{
  "type": "https://httpstatuses.io/400",
  "title": "Incompatible units",
  "status": 400,
  "detail": "Cannot convert between units from different categories ('Length' and 'Mass').",
  "instance": "/api/v1/conversions"
}
```

---

## Project structure

```
UnitConversion.sln
└── src/
    ├── UnitConversion.Domain/           # Pure domain types, no framework refs
    │   ├── Units/
    │   │   ├── Unit.cs                  # A unit + ToBase / FromBase delegates
    │   │   ├── UnitCategory.cs          # A category + its canonical base unit
    │   │   └── IUnitRegistry.cs         # Storage abstraction (swap-friendly)
    │   └── Exceptions/                  # Domain-level error types
    │
    ├── UnitConversion.Application/      # Business logic + DI composition
    │   ├── Conversions/
    │   │   ├── ConversionService.cs     # Performs the conversion
    │   │   ├── ConversionRequest/Result # DTOs
    │   │   └── ConversionRequestValidator.cs   # FluentValidation rules
    │   ├── Units/
    │   │   ├── IUnitSeed.cs             # Contribution seam for categories
    │   │   ├── SeededUnitRegistry.cs    # In-memory IUnitRegistry impl
    │   │   └── Seeds/                   # LengthUnitSeed, MassUnitSeed, TemperatureUnitSeed
    │   └── DependencyInjection.cs       # AddUnitConversion() extension
    │
    └── UnitConversion.Api/              # HTTP layer
        ├── Controllers/V1/              # CategoriesController, ConversionsController
        ├── Contracts/                   # API response DTOs (kept separate from domain)
        ├── Infrastructure/
        │   ├── ExceptionHandling/       # Maps exceptions to ProblemDetails
        │   └── Swagger/                 # Per-version Swagger document
        ├── Program.cs                   # Composition root
        └── appsettings*.json            # Serilog config etc.
```

The dependency direction is strict: `Api → Application → Domain`. Domain has no
references to anything else.

---

## Supported categories and units

| Category    | Base unit | Units                                                            |
|-------------|-----------|------------------------------------------------------------------|
| Length      | `m`       | `mm`, `cm`, `m`, `km`, `in`, `ft`, `yd`, `mi`, `nmi`             |
| Mass        | `kg`      | `mg`, `g`, `kg`, `t`, `oz`, `lb`, `st`                           |
| Temperature | `K`       | `K`, `C`, `F`, `R`                                               |

Each unit also accepts common aliases (`kilometers`, `inches`, `°C`,
`fahrenheit`, `lbs`, ...). See `GET /api/v1/categories/{category}/units` for
the full list.

---

## Adding a new unit or category

The design intentionally makes both operations local edits.

### Add a unit to an existing category

Open the relevant seed file (e.g. `src/UnitConversion.Application/Units/Seeds/LengthUnitSeed.cs`)
and add a single line:

```csharp
Unit.Linear("fur", "furlong", CategoryName, 201.168, new[] { "furlongs" }),
```

That's it — restart the API and the new unit is discoverable, listable, and
convertible.

### Add a brand-new category

1. Create a new seed class implementing `IUnitSeed` under
   `src/UnitConversion.Application/Units/Seeds/`. Choose a canonical base unit
   and express every other unit as a factor against it (or, for non-linear
   conversions, supply explicit `ToBase`/`FromBase` delegates — see
   `TemperatureUnitSeed` for the pattern).
2. Register it once in
   `src/UnitConversion.Application/DependencyInjection.cs`:

   ```csharp
   services.AddSingleton<IUnitSeed, MyNewCategorySeed>();
   ```

No other file needs to change.

### Replace the storage mechanism

`SeededUnitRegistry` is the only in-memory implementation today. To load units
from a database or a JSON configuration file, write a new `IUnitRegistry`
implementation and register it in DI instead. The controllers, the conversion
service, the validators, and the seed files are all unaware of where data
comes from.

---

## Design decisions and trade-offs

- **Layered solution (`Domain` / `Application` / `Api`).** Even for a small
  project, the seam between business logic and HTTP transport pays for itself
  the first time you want to reuse the conversion engine elsewhere (a CLI, a
  message handler, a benchmark). The Domain assembly has zero framework
  references on purpose.

- **Delegate-based `Unit`, with a `Unit.Linear(...)` shortcut.** Most units are
  multiplicative against a base unit, so the common case stays a one-liner.
  Temperature scales are affine, not linear (Celsius → Kelvin adds an offset),
  so each unit also lets you supply explicit `ToBase` / `FromBase` delegates.
  This avoids a parallel hierarchy of "linear vs. affine vs. lookup-table"
  converter classes and keeps the math uniform: `result =
  toUnit.FromBase(fromUnit.ToBase(value))`.

- **`IUnitRegistry` as a storage seam.** Today it's backed by an in-memory
  `SeededUnitRegistry` populated by hardcoded `IUnitSeed` contributors. When
  the dataset needs to scale to "hundreds of units" the doc anticipates,
  swapping in a database- or configuration-backed implementation is a single
  DI change.

- **API versioning from day one.** Routes are `/api/v{version}/...` and
  Swagger renders a separate document per version. Today there is only `v1`,
  but introducing `v2` later is purely additive.

- **`ProblemDetails` (RFC 7807) for every error.** Domain exceptions are
  translated by a centralised `IExceptionHandler` into consistent
  machine-readable bodies (status, title, detail, instance). Model-binding
  failures and FluentValidation failures both flow through the same shape,
  so clients have one error contract to deal with.

- **FluentValidation over `[DataAnnotations]`.** Validation rules live next to
  the request types, are unit-testable in isolation, and don't pollute the
  HTTP contract.

- **Serilog for structured logs.** Configured through `appsettings.json`, with
  per-request logging via `UseSerilogRequestLogging()`. The bootstrap logger
  ensures startup crashes are captured too.

- **Swagger UI exposed in every environment.** The challenge is meant to be
  poked at by a reviewer, so the Swagger UI is intentionally available outside
  of `Development` and the root URL redirects to it. In a production
  deployment you would gate this behind an environment check or auth.

- **.NET 8 (LTS).** Targeted because it is a long-term-supported release with
  broad tooling coverage. The project upgrades cleanly to .NET 9/10 — only
  the `<TargetFramework>` lines and a couple of pinned package versions
  (`Asp.Versioning.*`) need to change.

### What was deliberately left out

- **No automated tests.** The challenge does not require them, and the
  service layer is small enough to verify against the running API or the
  Swagger UI. The code is structured so that adding an
  `xUnit + WebApplicationFactory` test project later would touch nothing in
  the existing assemblies.
- **No CORS, no auth, no rate limiting, no health checks, no Docker / CI.**
  These are real-project concerns but well outside the scope of "convert a
  number between two units"; they would be added once the deployment
  environment is decided.

---

## Tech stack

- **.NET 8** (ASP.NET Core Web API, controllers)
- **FluentValidation** + `FluentValidation.DependencyInjectionExtensions`
- **Serilog** (`Serilog.AspNetCore`, console sink)
- **Asp.Versioning.Mvc** + `Asp.Versioning.Mvc.ApiExplorer` for API versioning
- **Swashbuckle.AspNetCore** + `Swashbuckle.AspNetCore.Annotations` for OpenAPI/Swagger UI
