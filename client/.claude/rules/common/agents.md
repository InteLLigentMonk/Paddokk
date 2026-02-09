# Agent Orchestration

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

1. Complex feature requests - Use **planner** agent
2. Code just written/modified - Use **code-reviewer** agent
3. Bug fix or new feature - Use **tdd-guide** agent
4. Architectural decision - Use **architect** agent

## Parallel Task Execution

ALWAYS use parallel Task execution for independent operations.

## Feature Implementation Workflow

1. **Plan** - Use **planner** agent to create implementation plan
2. **TDD** - Use **tdd-guide** agent: write tests first (RED), implement (GREEN), refactor (IMPROVE)
3. **Review** - Use **code-reviewer** agent, address CRITICAL and HIGH issues
4. **Commit** - Conventional commits: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`, `perf:`, `ci:`
