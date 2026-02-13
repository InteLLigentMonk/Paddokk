# Task-Based Quick Start Guide

Find the right documentation for what you want to accomplish.

**See also:** [INDEX.md](./INDEX.md) for complete file listing by category.

## Planning & Architecture

### Planning a New Feature

**Goal:** Create an implementation plan before writing code.

**Read (in order):**
1. [agents.md](./rules/common/agents.md) - Understand when to use planner vs architect
2. [planner.md](./agents/planner.md) OR [architect.md](./agents/architect.md) - Depending on complexity
3. [git-workflow.md](./rules/common/git-workflow.md) - Create feature branch first

**Agents to use:**
- `architect` - For major features affecting multiple systems (auth, routing, state management)
- `planner` - For complex features and refactoring (after architect if needed)

**Steps:**
1. Create feature branch: `git checkout -b feat/feature-name`
2. If major feature: Use architect agent to design system architecture
3. Use planner agent to create detailed implementation plan
4. Review plan and proceed with implementation

**Related skills:** `/feature-dev` for guided feature development workflow

---

### Making Architectural Decisions

**Goal:** Choose between technology options or design system architecture.

**Read (in order):**
1. [agents.md](./rules/common/agents.md) - When to use architect agent
2. [architect.md](./agents/architect.md) - Architectural decision workflow
3. [performance.md](./rules/common/performance.md) - Performance considerations

**Use architect agent when:**
- Adding major features (authentication, state management, routing)
- Choosing between technology options
- Refactoring large parts of codebase
- Performance optimization at system level
- Integration patterns for third-party services

**Steps:**
1. Use architect agent first to design architecture
2. Review architectural recommendations
3. Use planner agent to create implementation plan
4. Proceed with implementation

---

## Development Workflow

### Writing Tests First (TDD)

**Goal:** Follow test-driven development workflow for new features or bug fixes.

**Read (in order):**
1. [agents.md](./rules/common/agents.md) - TDD workflow overview
2. [tdd-guide.md](./agents/tdd-guide.md) - Detailed TDD process
3. [git-workflow.md](./rules/common/git-workflow.md) - Commit format for tests

**Agent to use:** `tdd-guide`

**Steps:**
1. Use tdd-guide agent to write tests first (RED)
2. Implement minimal code to pass tests (GREEN)
3. Refactor for quality (IMPROVE)
4. Use code-reviewer agent to review
5. Commit with `test:` or `feat:` type

**Requirements:**
- 80% minimum test coverage
- Unit tests for utilities
- Integration tests for APIs
- E2E tests for critical flows

---

### Reviewing Code Changes

**Goal:** Review code for quality, security, and best practices.

**Read (in order):**
1. [agents.md](./rules/common/agents.md) - Code review workflow
2. [code-reviewer.md](./agents/code-reviewer.md) - Review criteria
3. [security-reviewer.md](./agents/security-reviewer.md) - Security analysis (if needed)

**Agents to use:**
- `code-reviewer` - After writing/modifying code
- `security-reviewer` - Before commits for security analysis

**Steps:**
1. Complete your code changes
2. Use code-reviewer agent to analyze changes
3. Address CRITICAL and HIGH priority issues
4. For security-sensitive code: Use security-reviewer agent
5. Proceed with commit

**Related skills:** `/code-review` or `/review-pr` for PR-focused reviews

---

### Fixing Build Errors

**Goal:** Resolve compilation errors, type errors, or build failures.

**Read (in order):**
1. [agents.md](./rules/common/agents.md) - Build error workflow
2. [build-error-resolver.md](./agents/build-error-resolver.md) - Error resolution process
3. [performance.md](./rules/common/performance.md) - When build is slow

**Agent to use:** `build-error-resolver`

**Steps:**
1. Note the build error message
2. Use build-error-resolver agent with error details
3. Apply suggested fixes incrementally
4. Verify after each fix
5. Run tests to ensure no regressions

---

## UI Development

### Building UI Components

**Goal:** Create new UI components with Mantine and proper styling.

**Read (in order):**
1. [CLAUDE.md](../CLAUDE.md) - Tech stack and mobile-first approach
2. [mantine-dark-mode.md](./rules/common/mantine-dark-mode.md) - Dark mode patterns (CRITICAL)
3. [plugins.md](./rules/common/plugins.md) - Use Context7 for Mantine API docs

**Key principles:**
- **Mobile-first:** Always start with mobile layout (320px - 768px)
- **No conditional JSX:** Use `lightHidden`/`darkHidden` CSS props instead
- **SSR safe:** Never branch on `colorScheme` (causes hydration mismatch)

**Steps:**
1. Design mobile layout first
2. Use Context7 to verify Mantine component APIs
3. Implement with mobile-first responsive props
4. Test on mobile (320px), tablet (768px), desktop (1024px+)
5. Verify dark mode with `lightHidden`/`darkHidden` patterns

**Related skills:** `/frontend-design` for production-grade UI components

---

### Fixing Dark Mode Hydration Errors

**Goal:** Resolve "Hydration failed" errors related to color scheme.

**Read (in order):**
1. [mantine-dark-mode.md](./rules/common/mantine-dark-mode.md) - SSR dark mode patterns

**Critical fix:**
```tsx
// BAD - causes hydration mismatch
{computedColorScheme === "dark" ? <Moon /> : <Sun />}

// GOOD - render both, CSS controls visibility
<Box darkHidden><Sun /></Box>
<Box lightHidden><Moon /></Box>
```

**Common mistakes:**
- Conditional JSX based on `colorScheme` or `computedColorScheme`
- Combining `lightHidden`/`darkHidden` with `display` style prop
- Mismatched `defaultColorScheme` between `ColorSchemeScript` and `MantineProvider`

---

### Using Notifications

**Goal:** Show success, error, warning, or info messages to users.

**Read (in order):**
1. [notifications.md](./rules/common/notifications.md) - Usage patterns

**Usage:**
```typescript
// In React components
const notifications = useNotifications()
notifications.success({ message: "Saved!" })

// In non-React contexts
import { notify } from '@/integrations/mantine'
notify.error({ message: "Failed to save" })
```

**Automatic handling:** TanStack Query errors are automatically shown via global error handlers (see notifications.md).

---

## Database & Schema

### Managing Database Schema

**Goal:** Add/modify database tables for Better Auth plugins or app features.

**Read (in order):**
1. [database-schema.md](./rules/common/database-schema.md) - Better Auth + Drizzle workflow
2. [CLAUDE.md](../CLAUDE.md) - Database configuration

**When to use:**
- Adding Better Auth plugins (JWT, 2FA, passkey)
- Creating app-specific tables
- Modifying existing schema

**Steps:**
1. Update `src/lib/auth.ts` with new plugins
2. Run Better Auth CLI: `npx @better-auth/cli generate`
3. Diff `auth-schema.ts` against `src/lib/db/schema.ts`
4. Update `schema.ts` with new tables/columns (don't blindly replace)
5. Delete `auth-schema.ts`
6. Apply to database:
   - Development: `npm run db:push` (fast, no migration files)
   - Production: `npm run db:generate` + `npm run db:migrate`
7. Restart dev server and verify

---

## Git & Release Workflow

### Making Commits

**Goal:** Create properly formatted commits following conventional commits.

**Read (in order):**
1. [git-workflow.md](./rules/common/git-workflow.md) - Commit format and branching
2. [agents.md](./rules/common/agents.md) - Feature implementation workflow

**Commit format:**
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types:** `feat`, `fix`, `perf`, `refactor`, `chore`, `revert`, `docs`, `style`, `test`, `ci`

**Steps:**
1. Ensure on feature branch (not `main`)
2. Stage changes: `git add <files>`
3. Commit: `git commit -m "feat(scope): description"`
4. Or use `/commit` skill for guided commits

**Related skills:** `/commit` or `/commit-push-pr`

---

### Creating a Release

**Goal:** Bump version, generate changelog, and create git tag.

**Read (in order):**
1. [versioning.md](./rules/common/versioning.md) - SemVer rules and release workflow
2. [git-workflow.md](./rules/common/git-workflow.md) - Commit format (affects version bump)
3. [release.md](./commands/release.md) - Detailed release process

**Version bumping:**
- **MAJOR:** Breaking changes (`feat!` or `BREAKING CHANGE:`)
- **MINOR:** New features (`feat`)
- **PATCH:** Bug fixes (`fix`, `perf`, `refactor`)

**Steps (automatic via skill):**
1. Ensure on `main` branch with clean working tree
2. Use `/release` skill for guided workflow
3. Review commits since last tag
4. Preview version bump with dry-run
5. Confirm and create release
6. Push to remote with tags

**Steps (manual):**
1. `git checkout main && git pull`
2. `cd client && npm run release:dry` (preview)
3. `npm run release` (creates tag and commit)
4. `git push --follow-tags`

---

### Creating Pull Requests

**Goal:** Create a PR with proper title and description.

**Read (in order):**
1. [git-workflow.md](./rules/common/git-workflow.md) - PR workflow
2. [versioning.md](./rules/common/versioning.md) - Conventional commit format for PR title

**Steps:**
1. Push feature branch: `git push -u origin feat/feature-name`
2. Create PR with conventional commit title (e.g., `feat(auth): add password reset`)
3. Use PR description template if available (check `.github/PULL_REQUEST_TEMPLATE`)
4. Get code review
5. Use **squash merge** to `main` (keeps history clean)
6. Delete feature branch after merge

**Related skills:** `/commit-push-pr` for all-in-one workflow

---

## Tools & Plugins

### Finding the Right Tool

**Goal:** Understand which MCP server, skill, or agent to use for a task.

**Read (in order):**
1. [plugins.md](./rules/common/plugins.md) - Complete MCP and skills reference
2. [agents.md](./rules/common/agents.md) - Agent selection

**Decision table:**

| Task | Use |
|---|---|
| Look up library API docs | Context7 MCP server |
| GitHub operations (issues, PRs) | GitHub MCP server |
| Plan complex feature | `/feature-dev` skill OR `planner` agent |
| Review PR | `/review-pr` skill OR `code-reviewer` agent |
| Build UI components | `/frontend-design` skill |
| Fix build errors | `build-error-resolver` agent |
| TDD workflow | `tdd-guide` agent |
| Security analysis | `security-reviewer` agent |
| Git commit/PR | `/commit` or `/commit-push-pr` skill |
| Release | `/release` skill |

---

### Using Context7 for Library Docs

**Goal:** Look up current API documentation for project dependencies.

**Read (in order):**
1. [plugins.md](./rules/common/plugins.md) - Context7 usage
2. [agents.md](./rules/common/agents.md) - Proactive Context7 usage

**When to use:**
- Before implementing features using Mantine, TanStack, Tiptap, Better Auth, Zod
- Unsure about current API patterns
- Checking for breaking changes or migration guides

**Steps:**
1. Call `resolve-library-id` with library name (e.g., "mantine", "tanstack-router")
2. Call `query-docs` with library ID and specific question

**Proactive:** Always consult Context7 before writing code using third-party APIs you're not certain about.

---

### Configuring Permissions & Hooks

**Goal:** Set up auto-allowed commands and hooks in settings.local.json.

**Read (in order):**
1. [settings-hooks.md](./rules/common/settings-hooks.md) - Configuration documentation
2. [README.md](./README.md) - Overview of current setup

**Current configuration:**
- **Permissions:** Auto-allow npm, vitest, prettier, eslint, git, MCP plugins
- **Hooks:** PostToolUse Prettier hook (auto-format on Edit/Write)

**To recreate/modify:**
Use `/hookify` skill for interactive hook creation.

---

## Performance & Optimization

### Choosing the Right Model

**Goal:** Select the appropriate model for agents and tasks.

**Read (in order):**
1. [performance.md](./rules/common/performance.md) - Model selection strategy
2. [agents.md](./rules/common/agents.md) - Agent model assignments

**Model selection:**
- **Haiku 4.5:** Lightweight agents, frequent invocation, pair programming
- **Sonnet 4.5:** Main development, orchestration, complex coding (most agents use this)
- **Opus 4.5:** Architectural decisions, deep reasoning (architect, planner)

**Current agent assignments:**
- Opus: architect, planner
- Sonnet: code-reviewer, tdd-guide, build-error-resolver, security-reviewer, refactor-cleaner

---

### Managing Context Window

**Goal:** Optimize context usage for large tasks.

**Read (in order):**
1. [performance.md](./rules/common/performance.md) - Context window management
2. [agents.md](./rules/common/agents.md) - Parallel task execution

**Best practices:**
- Avoid last 20% of context for large refactoring or multi-file features
- Use extended thinking (enabled by default, up to 31,999 tokens)
- Enable Plan Mode for complex tasks requiring deep reasoning
- Use parallel Task execution for independent operations

**Context sensitivity:**
- **High:** Large-scale refactoring, multi-file features, complex debugging
- **Low:** Single-file edits, utilities, docs, simple fixes

---

## Common Workflows

### Full Feature Development Flow

**Complete workflow from planning to release:**

1. **Branch:** `git checkout -b feat/feature-name` ([git-workflow.md](./rules/common/git-workflow.md))
2. **Architect:** Use `architect` agent for major features ([architect.md](./agents/architect.md))
3. **Plan:** Use `planner` agent ([planner.md](./agents/planner.md))
4. **Research:** Use Context7 for library APIs ([plugins.md](./rules/common/plugins.md))
5. **TDD:** Use `tdd-guide` agent - write tests first ([tdd-guide.md](./agents/tdd-guide.md))
6. **Implement:** Build the feature
7. **Review:** Use `code-reviewer` agent ([code-reviewer.md](./agents/code-reviewer.md))
8. **Commit:** Follow conventional commits ([git-workflow.md](./rules/common/git-workflow.md))
9. **PR:** Create PR and merge to main ([git-workflow.md](./rules/common/git-workflow.md))
10. **Release:** Use `/release` skill ([versioning.md](./rules/common/versioning.md), [release.md](./commands/release.md))

---

## Quick Links

- **All files:** [INDEX.md](./INDEX.md)
- **Hub:** [README.md](./README.md)
- **Project config:** [../CLAUDE.md](../CLAUDE.md)
- **Settings:** [settings-hooks.md](./rules/common/settings-hooks.md)
