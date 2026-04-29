# Paddokk

JDM automotive community platform. Combining old school forums with modern social media.
Allows users to create cars, make journeys with said cars, follow other journeys and cars and comment.
Also gives user other nice to have functions related to the automotive hobby like a inventory list,
Marketplace, Stores, Harness-designer and more.

## Tech Stack

### Backend (.NET API)
- .NET 10, C#
- EF Core with PostgreSQL
- MediatR for CQRS (commands/queries)
- Clean Architecture: `Api/`, `Core/`, `Data/` layers
- `Core/` is organized by feature (e.g. `Core/Features/Cars/`, `Core/Features/Users/`)
- Each feature contains its own Commands, Queries, Handlers, DTOs, and Validators

### Frontend (TypeScript)
- TanStack Start (framework), TanStack Form, Query, Store
- Mantine UI components
- DND-Kit for drag-and-drop
- Orval for OpenAPI → TypeScript client generation
- BetterAuth for authentication
- Drizzle ORM (for BetterAuth's DB needs on the frontend/BFF side)
- Zod for validation schemas

### BFF Pattern
- TanStack Start server functions act as the BFF layer
- Server functions call the .NET API through Orval-generated TypeScript clients
- Frontend components never call the .NET API directly
- Flow: Component → TanStack Query → Server Function → Orval Client → .NET API

### Auth
- BetterAuth handles session management on the frontend/BFF
- .NET API validates tokens from BetterAuth
- Social logins: Facebook, Google
- Standard: email/password

### Hosting
- Frontend: Vercel
- Backend: Azure (App Service or Container Apps)
- Storage: Azure Blob Storage
- Database: PostgreSQL (Azure or external)

### DevOps
- GitHub Actions for CI/CD
- Docker for containerization
- Conventional commits (feat:, fix:, docs:, refactor:, test:, chore:)
- Semantic versioning

## Conventions

### .NET Backend
- Use MediatR `IRequest<T>` / `IRequestHandler<TRequest, TResult>` for all business logic
- Controllers in `Api/Controllers/` are thin — they only map HTTP to MediatR requests
- Controllers inherit from `ControllerBase`, use `[ApiController]` attribute
- Return `Ok()`, `NotFound()`, `BadRequest()`, etc. from action methods
- Use FluentValidation with MediatR pipeline behaviors for input validation
- Entity configurations go in `Data/Configurations/`
- Migrations: `dotnet ef migrations add <Name> --project Data --startup-project Api`
- Never put business logic in controllers/endpoints or in EF configurations

### Frontend
- Components use Mantine for UI, never raw HTML for standard controls
- Forms use TanStack Form with Zod schemas for validation
- All API calls go through server functions (BFF), never direct fetch to the .NET API
- Use TanStack Query for server state, TanStack Store for client state
- Package manager: pnpm (never use npm or yarn)
- Orval regeneration: `pnpm orval` (reads from orval.config.ts)

### General
- The overall language of the app is english until i18n is implemeted and supported
- Swedish comments/docs are OK, code and variable names always in English
- Prefer explicit types over `any` in TypeScript
- Prefer `record` types for DTOs in C#
- Tests: xUnit + FluentAssertions for backend, Vitest + Playwright for frontend
- Always follow TDD (red-green-refactor). See the tdd skill for the full workflow
- Always use the latest docs for reference material, get it with context7.
- Make shure to conform to OWASP top 10 for security.

## Plan Mode
- Make the plan extremely concise. Sacrifice grammar for the sake of concision.
- At the end of each plan, give me a list of unresolved questions to answer, if any.