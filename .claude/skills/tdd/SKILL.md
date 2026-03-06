---
name: tdd
description: Use when implementing any new feature, fixing a bug, or refactoring code. Enforces the red-green-refactor cycle — always write a failing test before writing implementation code.
model: sonnet
---

# Test-Driven Development

Every code change follows red-green-refactor. No exceptions.

## The Cycle

### 1. RED — Write a failing test
- Write the smallest test that describes the next piece of behavior
- Run it. It MUST fail. If it passes, the test isn't testing anything new
- Commit: `test(feature): add failing test for <behavior>`

### 2. GREEN — Make it pass
- Write the minimum code to make the test pass
- No cleverness, no optimization, no "while I'm here" changes
- Run all tests. They must ALL pass
- Commit: `feat(feature): implement <behavior>`

### 3. REFACTOR — Clean up
- Improve the code without changing behavior
- Run all tests after every change — they must stay green
- Extract methods, rename, remove duplication, improve readability
- Commit: `refactor(feature): <what you improved>`

Then repeat from step 1 for the next behavior.

## Backend Tests (xUnit + FluentAssertions)

### Project Structure
```
Tests/
  Unit/
    Features/
      Cars/
        CreateCarCommandHandlerTests.cs
        GetCarQueryHandlerTests.cs
  Integration/
    Endpoints/
      CarsControllerTests.cs
```

### Unit Test Pattern
Test MediatR handlers in isolation:

```csharp
public class CreateCarCommandHandlerTests
{
    private readonly AppDbContext _context;
    private readonly CreateCarCommandHandler _handler;

    public CreateCarCommandHandlerTests()
    {
        // Use in-memory or testcontainers PostgreSQL
        _context = TestDbContextFactory.Create();
        _handler = new CreateCarCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCarDto()
    {
        // Arrange
        var command = new CreateCarCommand("Nissan", "Skyline", 1999);

        // Arrange
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Make.Should().Be("Nissan");
        result.Model.Should().Be("Skyline");
        result.Year.Should().Be(1999);
    }

    [Fact]
    public async Task Handle_DuplicateCar_ThrowsException()
    {
        // Arrange
        _context.Cars.Add(new Car { Make = "Nissan", Model = "Skyline", Year = 1999 });
        await _context.SaveChangesAsync();
        var command = new CreateCarCommand("Nissan", "Skyline", 1999);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateEntityException>();
    }
}
```

### Naming Convention
`Method_Scenario_ExpectedResult`
- `Handle_ValidCommand_ReturnsCarDto`
- `Handle_InvalidYear_ThrowsValidationException`
- `Handle_NonExistentId_ReturnsNull`

### FluentAssertions Cheat Sheet
```csharp
// Basic
result.Should().Be(expected);
result.Should().NotBeNull();
result.Should().BeOfType<CarDto>();

// Strings
name.Should().StartWith("Nissan");
name.Should().Contain("line");

// Numbers
year.Should().BeGreaterThan(1989);
year.Should().BeInRange(1989, 2002);

// Collections
cars.Should().HaveCount(3);
cars.Should().Contain(c => c.Make == "Nissan");
cars.Should().BeInAscendingOrder(c => c.Year);
cars.Should().OnlyContain(c => c.Year > 1989);

// Exceptions
act.Should().ThrowAsync<NotFoundException>();
act.Should().ThrowAsync<ValidationException>()
   .WithMessage("*Year*");
```

### Running Backend Tests
```bash
dotnet test                           # Run all tests
dotnet test --filter "FullyQualifiedName~Cars"  # Run tests for a feature
dotnet test --filter "Category=Unit"  # Run only unit tests
```

## Frontend Tests

### Vitest (Unit/Component)
```typescript
import { describe, it, expect } from 'vitest'

describe('carUtils', () => {
  it('should format car name correctly', () => {
    const result = formatCarName('Nissan', 'Skyline', 1999)
    expect(result).toBe('1999 Nissan Skyline')
  })

  it('should throw for invalid year', () => {
    expect(() => formatCarName('Nissan', 'Skyline', -1)).toThrow()
  })
})
```

### Playwright (E2E)
```typescript
import { test, expect } from '@playwright/test'

test('user can add a new car', async ({ page }) => {
  await page.goto('/cars/new')
  await page.fill('[name="make"]', 'Nissan')
  await page.fill('[name="model"]', 'Skyline')
  await page.fill('[name="year"]', '1999')
  await page.click('button[type="submit"]')

  await expect(page.locator('.car-card')).toContainText('Nissan Skyline')
})
```

### Running Frontend Tests
```bash
pnpm test                  # Vitest unit tests
pnpm test:e2e              # Playwright e2e tests
```

## Rules

- Never write implementation code without a failing test first
- One behavior per test — if a test name has "and" in it, split it
- Tests are first-class code — refactor them too, keep them readable
- Don't test framework behavior (EF Core, Mantine, TanStack) — test YOUR logic
- Use Arrange-Act-Assert pattern, always with clear section comments
- Aim for fast unit tests, use integration/e2e tests sparingly for critical paths