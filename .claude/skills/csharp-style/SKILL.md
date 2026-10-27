---
name: csharp-style
description: Use when writing, reviewing, or refactoring C# / .NET code so it matches Eric's house style — brace placement, namespace style, project layering (Api / Service / Service.Contracts / Model), interface and DTO naming, and SQL conventions. Trigger on new .cs files, new projects or classes, "does this match my style", "clean this up", or before finishing any C# edit.
tools: Read, Grep, Glob, Edit, Write, Bash
---

You enforce and apply Eric's personal C# / .NET house style. This style deviates from Visual Studio and `dotnet new` defaults in specific, deliberate ways — the defaults are **wrong** here. Never "fix" the style back toward the template.

Reference implementation: `C:\Code\MockServer\MyCodingStyle` (the `MyCodingSytle.*` solution). When something is ambiguous, read those files and mirror them.

## Formatting

**Braces: K&R / same-line — always.** This is the single most-violated rule, because every C# tool defaults to Allman. Opening brace goes on the same line as the declaration, separated by one space. This applies to namespaces, classes, interfaces, methods, properties with bodies, `if`/`else`, `for`, `foreach`, `while`, `try`/`catch`, `switch`, and object initializers.

```csharp
namespace MyCodingSytle.Service {
    public class PlatformService : Contracts.IPlatformService {

        public TriggerResponse TriggerEvent(TriggerRequest request) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            return new TriggerResponse {
                Status = true
            };
        }

    }
}
```

Not this (VS default, do not produce):

```csharp
namespace MyCodingSytle.Service
{
    public class PlatformService : IPlatformService
    {
```

**Block-scoped namespaces, not file-scoped.** `namespace Foo {` with the whole file indented inside. Do not emit `namespace Foo;` even though .NET 8 templates do.

**Indentation:** 4 spaces, no tabs. Everything nests one level under the namespace.

**Breathing room inside type bodies.** Classes get a blank line after the opening brace and before the closing brace, so members sit visually inside the type:

```csharp
public class PlatformService : Contracts.IPlatformService {

    public TriggerResponse TriggerEvent(TriggerRequest request) {
        ...
    }

}
```

Small pure-DTO types may omit this if every member is a one-line auto-property; blank lines between properties are still fine.

**Braces are never omitted**, even for one-line `if` bodies.

**A method body is a block, never an expression body.** `=>` on a method hides work in an
argument list — `=> await Continue(x, y, await Begin(x, y), ct)` makes the order of two
awaits something you work out rather than read. Write the block, and name the intermediate
when there is one:

```csharp
public async Task<RunRecord> RunAsync(Integration integration, TriggerContext trigger, CancellationToken ct = default) {
    var record = await BeginAsync(integration, trigger);
    return await ContinueAsync(integration, trigger, record, ct);
}
```

This is methods only. A computed property stays `=>`, and so do constructors, operators,
indexers, accessors and lambdas.

## Solution layout

Four-project layering, referenced in one direction only:

```
{Root}.Api                 ASP.NET Core Web API — controllers, Program.cs, wiring
{Root}.Model               POCOs / DTOs only. No dependencies.
{Root}.Service.Contracts   Interfaces only. References Model.
{Root}.Service             Implementations. References Model + Service.Contracts.
```

Rules that fall out of this:

- **`Model` has zero project references.** Nothing goes in it but plain data.
- **Interfaces live in `.Service.Contracts`, never beside their implementation.** The consumer depends on the contract assembly; the implementation assembly is a runtime detail wired up in DI.
- **`.Api` depends on `.Service.Contracts` (+ `.Model`), and registers `.Service` in the container.** Controllers take the interface, never the concrete class.
- **`.Service` never references `.Api`.** No leaks upward.
- One file per public type. File name matches the type name.

Solution file is the modern `.slnx` XML format, one `<Project Path="..."/>` per line.

## Naming

- Service interfaces: `I{Area}Service` → `IPlatformService`. Implementation drops the `I`: `PlatformService`.
- Service methods are verb-first and describe the operation: `TriggerEvent`, not `DoTrigger` or `Trigger`.
- **Request/response DTO pairs per operation**: `{Operation}Request` / `{Operation}Response` (`TriggerRequest`, `TriggerResponse`). Every service method takes a single request object and returns a single response object — do not add loose scalar parameters or return bare primitives, even when there's only one field today. The wrapper is there so the contract can grow without breaking callers.
- PascalCase for types, methods, properties, and public members. camelCase for locals and parameters.
- Private fields: `_camelCase` with a leading underscore (`_logger`).
- No abbreviations in type or member names.
- **Preserve the existing root namespace spelling exactly**, including `MyCodingSytle` in that solution. It's the established identifier; do not silently "correct" it. If asked to rename it, do it as an explicit, complete rename across csproj files, namespaces, folders, and the `.slnx`.

## Project files (`.csproj`)

Match the existing shape exactly:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

- `net8.0`, `ImplicitUsings` enabled, `Nullable` enabled — on every project.
- Blank line after `<Project>`, between item groups, and before `</Project>`. Two-space indent in XML.
- Because implicit usings are on, **do not add `using System;`, `System.Linq`, `System.Collections.Generic`, `System.Threading.Tasks`, etc.** Only add usings the compiler actually needs. (The stock `TriggerResponse.cs` has a leftover template block of these — that's template residue, not the style; strip it if you touch the file.)
- Nullable is on, so annotate honestly: `string?` when it can be null, and don't paper over warnings with `!`.

## Code content

- Constructor injection for dependencies, assigned to a `private readonly` field. **Write the constructor out**: no primary constructors on classes (`class X(IFoo foo)`), so the dependencies are visible as fields and the constructor is a place to validate and to read. Positional `record`s keep their parameter list; that is their definition, not a constructor.
- Controllers stay thin: `[ApiController]`, `[Route("[controller]")]`, attribute-routed actions that delegate straight into a service interface.
- Prefer expression-bodied **properties** for trivial computed values (`public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);`). Methods do not get one — see the formatting rule above.
- Auto-properties for DTOs; no backing fields unless there's real logic.
- **Tuple, record or class**: a tuple is for something simple and local (two or three obvious values handed back from a private method). If it may need more detail later, or carries a flag whose meaning a caller has to remember, make it a named `record` (positional, one per file, so callers can still deconstruct it). A major component of the domain (`Quote`, `Policy`) is a `class`. A public method never returns a tuple whose members are anything but self-evident pairs.
- Leave a `// implement behavior here` comment plus `throw new NotImplementedException();` when scaffolding a member deliberately left unfinished — but only when the user asked for a stub. Never ship a silent no-op.
- Comments are sparse and explain *why*, not *what*. Do not narrate obvious code.

## Known template residue — do not treat as style

The reference solution still contains untouched `dotnet new` output. When you edit these, bring them into the style above; do not cite them as precedent:

- `Program.cs` — top-level statements with Allman `if (app.Environment.IsDevelopment())`.
- `WeatherForecastController.Get()` — Allman-braced object initializer.
- `TriggerRequest.cs` — still declares `class Class1`; the real DTO type is missing.
- `TriggerResponse.cs` — unused `System.*` usings.
- `.Api.csproj` — has no `ProjectReference` to `.Service.Contracts` / `.Service` yet, and `PlatformService` is not registered in DI.

If work touches any of these, fix them as part of the change and say so.

## SQL and database work

Any SQL, migration, EF configuration, or stored procedure this style covers must additionally follow the database standards in `C:\Users\EricR\.claude\CLAUDE.md` — singular ProperCase table names, `{TableName}Id` primary keys, `usp_{VERB}_{Entity}` procedures, `DECIMAL(18,2)` for money, `DATETIME2`, explicitly named constraints, full audit columns, `SET NOCOUNT ON`, no `NOLOCK`, no cursors, no scalar UDFs, no triggers, no `SELECT *`. Read that file before writing SQL rather than working from memory.

## How to operate

When **writing** code: produce it in this style directly. Don't write template-default code and then reformat.

When **reviewing** code: report concrete violations with `file:line`, showing the corrected form. Order by impact — layering and contract violations first, then naming, then formatting. Don't pad the list with nits if the architecture is wrong.

When **fixing** code: make the edits, then build to confirm nothing broke:

```
dotnet build C:\Code\MockServer\MyCodingStyle\MyCodingStyle.slnx
```

Report what you changed and anything you deliberately left alone.
