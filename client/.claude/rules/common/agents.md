---
scope: general
applies-to: [agents, workflow, feature-development, parallel-execution]
read-when: [planning-feature, choosing-agent, understanding-workflow]
---

# Agent Orchestration

See also: [plugins.md](./plugins.md) for skills, MCP servers, and when to use skills vs agents.

## Available Agents

| Agent | Model | Purpose | When to Use | Documentation |
|---|---|---|---|---|
| planner | opus | Implementation planning | Complex features, refactoring | [agents/planner.md](../../agents/planner.md) |
| architect | opus | System design | Architectural decisions | [agents/architect.md](../../agents/architect.md) |
| code-reviewer | sonnet | Code review | After writing code | [agents/code-reviewer.md](../../agents/code-reviewer.md) |
| tdd-guide | sonnet | Test-driven development | New features, bug fixes | [agents/tdd-guide.md](../../agents/tdd-guide.md) |
| build-error-resolver | sonnet | Fix build errors | When build fails | [agents/build-error-resolver.md](../../agents/build-error-resolver.md) |
| security-reviewer | sonnet | Security analysis | Before commits | [agents/security-reviewer.md](../../agents/security-reviewer.md) |
| refactor-cleaner | sonnet | Dead code cleanup | Code maintenance | [agents/refactor-cleaner.md](../../agents/refactor-cleaner.md) |

**Quick reference:** See [INDEX.md](../../INDEX.md) for complete documentation map. See [performance.md](./performance.md) for model selection strategy.

## Immediate Agent Usage

No user prompt needed:

1. **Complex feature requests** - Use **planner** agent
2. **Code just written/modified** - Use **code-reviewer** agent
3. **Bug fix or new feature** - Use **tdd-guide** agent
4. **Architectural decision** - Use **architect** agent when:
   - Adding new major features that affect multiple systems (e.g., authentication, state management, routing)
   - Choosing between technology options (e.g., database, state library, API client)
   - Refactoring large parts of the codebase (e.g., folder restructure, component architecture)
   - Performance optimization at system level (e.g., caching strategy, bundle optimization)
   - Scalability concerns (e.g., handling large datasets, real-time features)
   - Integration patterns (e.g., connecting frontend to backend, third-party services)
   - State management architecture decisions
   - Data flow and component communication patterns
   - API design and endpoint structure
   - **Before** implementing major features - consult architect first, then use planner

## Proactive Tool Usage

- **Context7** - Always consult before writing code against third-party library APIs (Mantine, TanStack, Tiptap, Zod, Better Auth). Prefer verified docs over guessing.
- **GitHub MCP** - Use for all GitHub operations (issues, PRs, code search) instead of `gh` CLI when possible.

## Parallel Task Execution

ALWAYS use parallel Task execution for independent operations.

## Feature Implementation Workflow

0. **Branch** - Create feature branch from `main` (e.g., `feat/user-profile`, `fix/auth-redirect`). See [git-workflow.md](./git-workflow.md) for branching strategy.
1. **Architect** (for major features only) - Use **architect** agent to design system architecture ([agents/architect.md](../../agents/architect.md))
2. **Plan** - Use **planner** agent to create implementation plan ([agents/planner.md](../../agents/planner.md))
3. **Research** - Use **Context7** to verify library APIs needed for the feature (see [plugins.md](./plugins.md))
4. **TDD** - Use **tdd-guide** agent: write tests first (RED), implement (GREEN), refactor (IMPROVE) ([agents/tdd-guide.md](../../agents/tdd-guide.md))
5. **Review** - Use **code-reviewer** agent, address CRITICAL and HIGH issues ([agents/code-reviewer.md](../../agents/code-reviewer.md))
6. **Commit** - Conventional commits: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`, `perf:`, `ci:` (enforced by commitlint, see [git-workflow.md](./git-workflow.md))
