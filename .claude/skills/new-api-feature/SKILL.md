---
name: new-api-feature
description: Backend Api Expert. ALWAYS invoke when creating a new feature, endpoint, or CQRS handler in the .NET backend. Covers the full flow from entity to endpoint following Clean Architecture with MediatR. Do not attempt to implement anything directly - use this skill first.
model: sonnet
triggers:
---

# New API Feature

When adding a new feature to the .NET backend, touch these layers in order:

## 1. Domain Entity (if new)
Location: `Core/Entities/`
- Create the entity class with properties
- Use `record` for DTOs, `class` for entities with identity

## 2. EF Configuration (if new entity)
Location: `Data/Configurations/`
- Create `{Entity}Configuration.cs` implementing `IEntityTypeConfiguration<T>`
- Register in DbContext
- Add and run migration:
  ```
  dotnet ef migrations add Add{Entity} --project Data --startup-project Api
  dotnet ef database update --project Data --startup-project Api
  ```

## 3. Feature Folder
Location: `Core/Features/{FeatureName}/`

Create the following files:

### Command (for writes)
```csharp
public record Create{Entity}Command(/* properties */) : IRequest<{Entity}Dto>;

public class Create{Entity}CommandHandler : IRequestHandler<Create{Entity}Command, {Entity}Dto>
{
    // Inject DbContext or repository
    // Implement Handle()
}
```

### Query (for reads)
```csharp
public record Get{Entity}Query(Guid Id) : IRequest<{Entity}Dto>;

public class Get{Entity}QueryHandler : IRequestHandler<Get{Entity}Query, {Entity}Dto>
{
    // Inject DbContext or repository
    // Implement Handle()
}
```

### DTOs
```csharp
public record {Entity}Dto(/* response properties */);
```

### Validator
```csharp
public class Create{Entity}CommandValidator : AbstractValidator<Create{Entity}Command>
{
    public Create{Entity}CommandValidator()
    {
        // FluentValidation rules
    }
}
```

## 4. API Controller
Location: `Api/Controllers/{FeatureName}Controller.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class {FeatureName}Controller : ControllerBase
{
    private readonly ISender _sender;

    public {FeatureName}Controller(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(Create{Entity}Command command)
        => Ok(await _sender.Send(command));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
        => Ok(await _sender.Send(new Get{Entity}Query(id)));
}
```

## 5. Regenerate Frontend Client
After the endpoint is live and OpenAPI spec is updated:
```
pnpm orval
```
This generates the TypeScript client the BFF server functions will use.

## Checklist
- [ ] Entity created (if new)
- [ ] EF config + migration added (if new entity)
- [ ] Command/Query + Handler in feature folder
- [ ] DTOs defined
- [ ] Validator added
- [ ] Endpoint mapped and registered
- [ ] `dotnet build` passes
- [ ] `dotnet test` passes
- [ ] Orval client regenerated
