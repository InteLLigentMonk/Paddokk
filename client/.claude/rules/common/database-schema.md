# Database Schema Management

Better Auth generates the authoritative schema definition. Drizzle manages the database.

## Architecture

```
Better Auth config (auth.ts)     ← Source of truth for auth tables
        ↓
Better Auth CLI generate         ← Generates Drizzle table definitions
        ↓
src/lib/db/schema.ts             ← Single schema file used by app + Drizzle Kit
        ↓
Drizzle Kit (push / migrate)     ← Applies changes to PostgreSQL
```

## Adding or Changing Better Auth Plugins

When you add, remove, or reconfigure plugins in `src/lib/auth.ts`:

### Step 1: Generate schema from Better Auth

```bash
BETTER_AUTH_DB_CONNECTION_STRING="..." npx @better-auth/cli generate
```

This creates `auth-schema.ts` in the project root. It reflects ALL tables required by your current auth config and plugins.

### Step 2: Diff and update schema.ts

Compare `auth-schema.ts` against `src/lib/db/schema.ts`:

- Add new tables (e.g., `jwks` for JWT plugin, `twoFactor` for 2FA plugin)
- Add new columns to existing tables
- Do NOT blindly replace schema.ts -- it may contain custom columns or app-specific tables

### Step 3: Delete auth-schema.ts

```bash
rm auth-schema.ts
```

This file is generated output; do not commit it.

### Step 4: Apply to database

**Development (fast, no migration files):**

```bash
npm run db:push
```

`db:push` diffs the schema against the live database and applies only what changed. No migration files are created. Use this for rapid iteration.

**Production / tracked changes (migration files):**

```bash
npm run db:generate    # Creates SQL migration in drizzle/
npm run db:migrate     # Applies pending migrations
```

`db:generate` creates an incremental migration based on what changed since the last migration. `db:migrate` runs it. Use this when you need a reviewable migration history.

### Step 5: Verify

Restart the dev server. The Drizzle adapter error should be gone.

## Commands Reference

| Command               | Script                                             | Purpose                                |
| --------------------- | -------------------------------------------------- | -------------------------------------- |
| `npm run db:push`     | `dotenv -e .env.local -- drizzle-kit push --force` | Sync schema to DB (no migration files) |
| `npm run db:generate` | `dotenv -e .env.local -- drizzle-kit generate`     | Create migration SQL from schema diff  |
| `npm run db:migrate`  | `dotenv -e .env.local -- drizzle-kit migrate`      | Run pending migrations                 |
| `npm run db:studio`   | `dotenv -e .env.local -- drizzle-kit studio`       | Open Drizzle Studio (DB browser)       |

## When to Use Push vs Migrate

| Scenario                                         | Use                                        |
| ------------------------------------------------ | ------------------------------------------ |
| Local development, rapid iteration               | `db:push`                                  |
| Adding a plugin table during dev                 | `db:push`                                  |
| Production deployment                            | `db:migrate`                               |
| CI/CD pipeline                                   | `db:migrate`                               |
| Team needs to review schema changes              | `db:generate` + PR review + `db:migrate`   |
| Database already has tables, no prior migrations | `db:push` (avoids "already exists" errors) |

## Common Errors

### "The model X was not found in the schema object"

A Better Auth plugin requires a table that is missing from `schema.ts`. Fix:

1. Run Better Auth CLI generate (Step 1 above)
2. Copy the missing table definition to `schema.ts`
3. Run `db:push` or `db:generate` + `db:migrate`

### "relation X already exists" during migrate

The migration tries to CREATE tables that already exist (e.g., tables were created via `db:push` before migrations were set up). Fix:

- Use `db:push` instead (it only applies diffs)
- Or delete the stale migration file and re-generate from current state

### Better Auth CLI fails with missing env var

The CLI needs the database connection string. Either export it or use dotenv:

```bash
BETTER_AUTH_DB_CONNECTION_STRING="..." npx @better-auth/cli generate
```

## Plugin-to-Table Mapping

| Better Auth Plugin | Required Tables                      |
| ------------------ | ------------------------------------ |
| (core)             | user, session, account, verification |
| `jwt()`            | jwks                                 |
| `twoFactor()`      | twoFactor                            |
| `passkey()`        | passkey                              |
| `username()`       | (adds columns to user)               |
| `magicLink()`      | (uses verification)                  |
| `openAPI()`        | (no additional tables)               |

Always verify with `npx @better-auth/cli generate` when unsure.

## File Locations

- Auth config: `src/lib/auth.ts`
- Schema: `src/lib/db/schema.ts`
- DB client: `src/lib/db/index.ts`
- Drizzle config: `drizzle.config.ts`
- Migrations: `drizzle/`
- Env vars: `.env.local` (`BETTER_AUTH_DB_CONNECTION_STRING`)
