# Paddokk API

The .NET 10 backend for Paddokk. Clean Architecture with `Api/`, `Core/`, `Data/` layers, MediatR-based CQRS, EF Core + PostgreSQL.

For overall project context (frontend, BFF, conventions), see the repo root [CLAUDE.md](../CLAUDE.md).

---

## Prerequisites

| Tool | Version | Notes |
| --- | --- | --- |
| .NET SDK | 10.0 | Required — see `global.json` if present, otherwise `Paddokk.Api.csproj` `<TargetFramework>net10.0</TargetFramework>` |
| PostgreSQL | 14+ | Local instance or remote. Default dev expects a reachable Postgres server. |
| `dotnet-ef` tool | latest matching SDK | `dotnet tool install --global dotnet-ef` |
| IDE | Visual Studio 2022 / Rider / VS Code with C# Dev Kit | Any works |
| Git | any | |

Optional but recommended:

- A REST client (Scalar UI is auto-served at `/scalar/v1` in Development — no separate tool needed).
- `pgAdmin` or `DBeaver` for inspecting the DB.

---

## First-time setup

### 1. Clone and restore

```powershell
git clone https://github.com/InteLLigentMonk/Paddokk.git
cd Paddokk
dotnet restore API/Paddokk.sln
```

### 2. Configure the database

The dev `appsettings.Development.json` ships with a connection string pointing at the team's shared dev Postgres. If you want a fully local DB instead:

1. Create a database in your local Postgres: `CREATE DATABASE paddokk;`
2. Override the connection string via user-secrets (see step 4 below) — use the key `ConnectionStrings:DefaultConnection`.

Migrations run automatically on startup (see [Program.cs](Paddokk.Api/Program.cs)), so you don't need to apply them manually before the first run.

### 3. Bootstrap your secret store

`dotnet user-secrets` keeps real secret values out of `appsettings.json`. The `UserSecretsId` is already set in `Paddokk.Api.csproj`, so `init` is a no-op — you can skip straight to setting values.

The secrets store lives at `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json` on Windows (`~/.microsoft/usersecrets/<id>/secrets.json` on macOS/Linux). It is never committed.

### 4. Provide the required secret values

Run from `API/Paddokk.Api/`:

```powershell
cd API/Paddokk.Api

# Azure Blob Storage — connection string for the storage account hosting car images, post images, and avatars
dotnet user-secrets set "AzureStorage:ConnectionString" "<your-azure-blob-connection-string>"

# Resend API key — for transactional email
dotnet user-secrets set "Email:Resend:ApiKey" "<your-resend-api-key>"
```

Optional overrides (only if you want them different from `appsettings.Development.json`):

```powershell
# Custom local DB
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=paddokk;Username=postgres;Password=postgres"
```

Verify what you've set:

```powershell
dotnet user-secrets list
```

> Values printed by `dotnet user-secrets list` are in cleartext. Don't screen-share while running it.

For values you do not have, ask another team member out-of-band — never paste them into chat, issues, or PRs.

### 5. Run the API

```powershell
dotnet run --project API/Paddokk.Api
```

You should see logs like `Now listening on: http://localhost:5037`. The first run will create+seed the database.

Endpoints:

- API base: `http://localhost:5037`
- Scalar UI (Development only): `http://localhost:5037/scalar/v1`
- OpenAPI spec: `http://localhost:5037/openapi/v1.json`
- Health check: `http://localhost:5037/health`

Scalar auto-injects a Bearer token from `Development:BearerToken` in `appsettings.Development.json` if present, so you can call authenticated endpoints without manually pasting a token. Generate your own short-lived token from the frontend's BetterAuth flow if the default has expired.

---

## Common development tasks

### Adding a migration

```powershell
dotnet ef migrations add <Name> --project API/Paddokk.Data --startup-project API/Paddokk.Api
```

Migrations are auto-applied on app startup, so a restart is enough — no manual `database update` step.

### Running tests

```powershell
dotnet test API/Paddokk.Tests
```

Test stack: xUnit + FluentAssertions. Follow TDD (red-green-refactor) for new features — see the `tdd` skill if available.

### Regenerating the OpenAPI spec for the frontend

In Development, the API automatically writes `client/swagger.json` on startup. After backend changes:

1. Restart the API.
2. From the `client/` directory: `pnpm orval` (regenerates the fetch SDK and Zod schemas).

### Hitting an authenticated endpoint via Scalar

1. Sign in via the frontend (`pnpm dev` in `client/`) to populate a session cookie.
2. Grab the JWT from the frontend's BetterAuth `/api/auth/token` endpoint (or your own debug route).
3. Paste it into `Development:BearerToken` in `appsettings.Development.json`.
4. Restart the API. Scalar will auto-attach the token.

---

## Configuration & secrets reference

### Files

| File | Tracked in git? | Purpose |
| --- | --- | --- |
| `Paddokk.Api/appsettings.json` | No (gitignored) | Non-secret defaults + empty placeholders for keys provided via user-secrets / env vars. |
| `Paddokk.Api/appsettings.Development.json` | No (gitignored) | Dev-only overrides (frontend URL, dev DB, dev Scalar token). |
| `Paddokk.Api/Paddokk.Api.csproj` (`UserSecretsId`) | Yes | Identifier linking the project to its secrets store. |

Both `appsettings*.json` files are intentionally gitignored — they exist on every dev's disk but are not shared via git.

### Required secret keys

| Key | Where to set in dev | Where to set in prod |
| --- | --- | --- |
| `AzureStorage:ConnectionString` | `dotnet user-secrets` | Env var `AzureStorage__ConnectionString` |
| `Email:Resend:ApiKey` | `dotnet user-secrets` | Env var `Email__Resend__ApiKey` |

### Production env var binding

ASP.NET Core's default configuration provider loads environment variables at higher precedence than `appsettings.json`, using `__` (double underscore) as the section separator. Set the env vars listed above on the Azure App Service (or container env), and no `appsettings.Production.json` file is needed.

Key Vault references are deferred to a separate ops task — for now, plain env vars are the supported prod binding.

---

## Architecture overview

Three projects in `API/Paddokk.sln`:

- **`Paddokk.Api`** — ASP.NET Core host. Thin controllers in `Controllers/` that map HTTP to MediatR requests. Middleware in `Middleware/`. DI wiring in `Extensions/`.
- **`Paddokk.Core`** — Feature modules under `Features/<FeatureName>/` containing Commands, Queries, Handlers, DTOs, and Validators. All business logic lives here.
- **`Paddokk.Data`** — EF Core `DbContext`, entity configurations in `Configurations/`, repositories, and migrations.

See [CLAUDE.md](../CLAUDE.md) for conventions (MediatR, FluentValidation, JSON strict mode, etc.).

---

## Troubleshooting

**App fails to start with `Cannot connect to database`** — check that Postgres is running and the connection string in `appsettings.Development.json` (or your user-secrets override) is reachable.

**App starts but blob/email calls fail** — confirm you've run the `dotnet user-secrets set` commands and `dotnet user-secrets list` shows the keys.

**Scalar doesn't show endpoints** — make sure you're running with `ASPNETCORE_ENVIRONMENT=Development`. Scalar and `/openapi/v1.json` are only mapped in Development.

**`pnpm orval` fails after a backend change** — the API needs to be running so Orval can fetch the spec. Start the API first, then run `pnpm orval` from `client/`.