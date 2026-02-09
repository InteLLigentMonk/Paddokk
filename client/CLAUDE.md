# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
npm run dev          # Dev server on port 3000
npm run build        # Production build
npm run preview      # Preview production build
npm run test         # Run all tests (vitest run)
npm run lint         # ESLint
npm run format       # Prettier
npm run check        # Auto-format + auto-fix lint (prettier --write . && eslint --fix)
```

Run a single test file: `npx vitest run src/path/to/file.test.ts`

## Project Overview

Paddokk is a SaaS social platform for car enthusiasts. It aims to bridge the gap between old school
forums with proper threads and modern image and quick-message focused social platforms like instagram
or facebook.
The platform is centered around Journeys. A member can add their car/cars and make journeys with them
whether it be a new enginebuild or a roadtrip.

## Tech Stack

- **Framework:** TanStack Start (SSR via Nitro)
- **Routing:** TanStack Router (file-based, auto-generated route tree)
- **Data fetching:** TanStack Query (with SSR integration via react-router-ssr-query)
- **Forms:** TanStack Form
- **State:** TanStack Store (with Derived for computed state)
- **UI:** Mantine v8 (full suite: core, dates, modals, notifications, spotlight, carousel, dropzone, tiptap)
- **Styling:** Tailwind CSS v4 (Vite plugin) + Mantine PostCSS preset
- **Auth:** Better Auth (stateless mode, no DB yet)
- **Validation:** Zod v4
- **Icons:** lucide-react
- **Rich text:** Tiptap v3
- **API generation:** Orval (installed, not yet configured)
- **Testing:** Vitest + @testing-library/react + jsdom

## Architecture

### Routing & SSR

File-based routing in `src/routes/`. TanStack Router auto-generates `src/routeTree.gen.ts` — never edit this file manually.

Root layout in `src/routes/__root.tsx` defines the HTML shell (`shellComponent`), meta tags, CSS imports, and TanStack DevTools panel. The router context provides `QueryClient` for SSR query integration.

Router is created in `src/router.tsx` via `getRouter()` which wires up the route tree, React Query context, and SSR query integration.

API routes live under `src/routes/api/` (e.g., `src/routes/api/auth/$.ts` is the Better Auth catch-all handler).

### Key Directories

- `src/routes/` — Pages and API routes (file-based routing)
- `src/components/` — Shared UI components
- `src/hooks/` — Custom React hooks
- `src/lib/` — App utilities and config (auth, stores)
- `src/integrations/` — Third-party library wrappers (tanstack-query provider/devtools, better-auth UI)
- `src/data/` — Data utilities

### Path Alias

`@/*` maps to `src/*` (configured in both tsconfig.json and vite.config.ts).

### Styling

Tailwind CSS v4 for utilities, Mantine for component library. PostCSS config (`postcss.config.cjs`) includes `postcss-preset-mantine` and Mantine breakpoint variables. Both coexist — use Mantine components for complex UI, Tailwind for layout/utility classes.

### Design Principles

**Mobile-First Approach:** Paddokk is a mobile-first application. Always design and develop with mobile as the primary target, then progressively enhance for tablet and desktop. When implementing features or components:

- Start with mobile layout (320px - 768px)
- Test responsive behavior on tablet (768px - 1024px)
- Verify desktop experience (1024px+)
- Ensure touch targets are at least 44x44px for mobile usability
- Use Mantine's responsive props and Tailwind's responsive utilities (`sm:`, `md:`, `lg:`)
- Prioritize performance on mobile networks (lazy loading, optimized images)

Mantine breakpoints (from `postcss.config.cjs`):

- `xs`: 36em (576px)
- `sm`: 48em (768px)
- `md`: 62em (992px)
- `lg`: 75em (1200px)
- `xl`: 88em (1408px)

### Auth

Better Auth configured in `src/lib/auth.ts` (server) and `src/lib/auth-client.ts` (client). Currently stateless — needs a database adapter (Prisma/Drizzle + `npx @better-auth/cli migrate`) to persist users. Environment variables in `.env.local`: `BETTER_AUTH_URL`, `BETTER_AUTH_SECRET`.

### Notifications

Mantine notifications are integrated globally via `src/integrations/mantine/`. The system provides:

**Usage in React Components:**

```typescript
import { useNotifications } from "@/integrations/mantine";

function MyComponent() {
  const notifications = useNotifications();

  const handleAction = () => {
    notifications.success({ message: "Changes saved!" });
    notifications.error({ message: "Failed to save" });
    notifications.warning({ message: "Check your input" });
    notifications.info({ message: "Data loaded" });
  };
}
```

**Usage in Non-React Contexts:**

```typescript
import { notify } from '@/integrations/mantine'

async function apiCall() {
  try {
    await fetch(...)
    notify.success({ message: 'Data loaded' })
  } catch (error) {
    notify.error({ message: error.message })
  }
}
```

**Automatic Error Handling:**

TanStack Query mutations and queries automatically show error notifications via global error handlers in `src/integrations/tanstack-query/error-handler.ts`. Errors from queries/mutations are displayed automatically unless:

- The error is an `AbortError` (intentionally cancelled)
- A custom `onError` handler is provided (overrides global handler)

**Override Automatic Notifications:**

```typescript
const mutation = useMutation({
  mutationFn: api.updateUser,
  onError: (error) => {
    // Custom handling overrides global error notification
    notifications.warning({ message: "Please check your input" });
  },
});
```

**Notification Options:**

All notification methods accept:

- `message` (required): The notification message
- `title` (optional): Custom title (defaults: Success, Error, Warning, Info)
- `autoClose` (optional): Auto-close delay in ms or `false` to disable (default: 4000)
- `withCloseButton` (optional): Show/hide close button (default: true)

### 1. Code Organization

- Many small files over few large files
- High cohesion, low coupling
- 200-400 lines typical, 800 max per file
- Organize by feature/domain, not by type

### 2. Code Style

- No emojis in code, comments, or documentation
- Immutability always - never mutate objects or arrays
- No console.log in production code
- Proper error handling with try/catch
- Input validation with Zod or similar
- No semicolons
- Single quotes
- Trailing commas
- Strict TypeScript (`noUnusedLocals`, `noUnusedParameters`)

### 3. Testing

- TDD: Write tests first
- 80% minimum coverage
- Unit tests for utilities
- Integration tests for APIs
- E2E tests for critical flows

### 4. Security

- No hardcoded secrets
- Environment variables for sensitive data
- Validate all user inputs
- Parameterized queries only
- CSRF protection enabled
