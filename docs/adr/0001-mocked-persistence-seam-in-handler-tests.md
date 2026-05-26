# Repository interfaces stay as the test seam in handler tests

Handler tests in `Paddokk.Tests` mock `ICarRepository` / `IUserRepository` / `IUnitOfWork` via NSubstitute. The repository modules look shallow under the deletion test (1:1 over `DbContext`), but their interface IS the test seam — without it, handler tests need a real database.

We considered swapping to Testcontainers + Postgres + Respawn for higher-fidelity tests. Rejected on cost/speed grounds: fast mocked unit tests over slower integration-style tests, with the known trade-off that EF translation bugs (Include semantics, nullable nav props, projection failures) won't be caught at this layer.

## Consequences

- Future architecture reviews should NOT re-suggest deleting the repository layer or moving handler tests to real DB unless this decision is revisited.
- Projections-at-the-query-seam (mapper deepening) is also off the table — `.Select(...)` fragments can't be exercised through NSubstitute.
- Integration-flavoured tests, if needed, belong in a separate suite that bypasses the repository mocks.
