# Agent Orchestration

See also: `plugins.md` for skills, MCP servers, and when to use skills vs agents.

## Available Agents

| Agent                | Model  | Purpose                 | When to Use                   |
| -------------------- | ------ | ----------------------- | ----------------------------- |
| planner              | opus   | Implementation planning | Complex features, refactoring |
| architect            | opus   | System design           | Architectural decisions       |
| code-reviewer        | sonnet | Code review             | After writing code            |
| tdd-guide            | sonnet | Test-driven development | New features, bug fixes       |
| build-error-resolver | sonnet | Fix build errors        | When build fails              |
| security-reviewer    | sonnet | Security analysis       | Before commits                |
| refactor-cleaner     | sonnet | Dead code cleanup       | Code maintenance              |

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

1. **Architect** (for major features only) - Use **architect** agent to design system architecture
2. **Plan** - Use **planner** agent to create implementation plan
3. **Research** - Use **Context7** to verify library APIs needed for the feature
4. **TDD** - Use **tdd-guide** agent: write tests first (RED), implement (GREEN), refactor (IMPROVE)
5. **Review** - Use **code-reviewer** agent, address CRITICAL and HIGH issues
6. **Commit** - Conventional commits: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`, `perf:`, `ci:`
