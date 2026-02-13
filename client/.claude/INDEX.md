# Documentation Index

Complete reference for all Claude Code documentation in the Paddokk project.

## Quick Navigation

- **Task-based guide:** See [SCENARIOS.md](./SCENARIOS.md) for "I want to do X, what should I read?"
- **Getting started:** Read [README.md](./README.md) for overview and structure
- **Project-level config:** See [CLAUDE.md](../CLAUDE.md) for tech stack and architecture

## All Documentation Files (18 files)

### Agents (7 files)

Specialized sub-agents for complex tasks. All agents use YAML frontmatter for metadata.

| Agent | Model | Purpose | File |
|---|---|---|---|
| architect | opus | System design and architectural decisions | [agents/architect.md](./agents/architect.md) |
| planner | opus | Implementation planning for complex features | [agents/planner.md](./agents/planner.md) |
| build-error-resolver | sonnet | Fix build errors and compilation issues | [agents/build-error-resolver.md](./agents/build-error-resolver.md) |
| code-reviewer | sonnet | Code review for quality and security | [agents/code-reviewer.md](./agents/code-reviewer.md) |
| refactor-cleaner | sonnet | Dead code cleanup and refactoring | [agents/refactor-cleaner.md](./agents/refactor-cleaner.md) |
| security-reviewer | sonnet | Security vulnerability analysis | [agents/security-reviewer.md](./agents/security-reviewer.md) |
| tdd-guide | sonnet | Test-driven development workflow | [agents/tdd-guide.md](./agents/tdd-guide.md) |

**When to use:** See [rules/common/agents.md](./rules/common/agents.md) for agent selection decision table.

### Commands (1 file)

Skills invoked via slash commands (e.g., `/release`).

| Command | Skill | Purpose | File |
|---|---|---|---|
| release | `/release` | Guided semantic version release with changelog | [commands/release.md](./commands/release.md) |

**Note:** Other skills are documented in [rules/common/plugins.md](./rules/common/plugins.md).

### Rules (8 files)

Project-specific rules and guidelines loaded into every conversation.

| Rule | Scope | Purpose | File |
|---|---|---|---|
| agents | general | Agent orchestration, parallel execution, feature workflow | [rules/common/agents.md](./rules/common/agents.md) |
| database-schema | project | Better Auth + Drizzle schema management | [rules/common/database-schema.md](./rules/common/database-schema.md) |
| git-workflow | general | Branching strategy, conventional commits | [rules/common/git-workflow.md](./rules/common/git-workflow.md) |
| mantine-dark-mode | project | SSR dark mode, hydration fix patterns (CRITICAL) | [rules/common/mantine-dark-mode.md](./rules/common/mantine-dark-mode.md) |
| notifications | project | Mantine notifications usage patterns | [rules/common/notifications.md](./rules/common/notifications.md) |
| performance | general | Model selection, context window management | [rules/common/performance.md](./rules/common/performance.md) |
| plugins | general | MCP servers, skills, when to use what | [rules/common/plugins.md](./rules/common/plugins.md) |
| versioning | general | SemVer, release workflow, changelog generation | [rules/common/versioning.md](./rules/common/versioning.md) |

**Scope Legend:**
- **general** - Applicable to most projects, reusable patterns
- **project** - Paddokk-specific configuration and architecture

### Configuration (2 files)

| File | Purpose |
|---|---|
| [README.md](./README.md) | Hub for .claude/ directory, structure overview |
| [settings.local.json](./settings.local.json) | Permissions and hooks config |

**Settings documentation:** See [rules/common/settings-hooks.md](./rules/common/settings-hooks.md) for how to configure permissions and hooks.

## Quick Reference Tables

### When to Read What

| I want to... | Read these files |
|---|---|
| Plan a complex feature | [agents.md](./rules/common/agents.md) → [planner.md](./agents/planner.md) |
| Make a release | [versioning.md](./rules/common/versioning.md) → [release.md](./commands/release.md) |
| Fix build errors | [agents.md](./rules/common/agents.md) → [build-error-resolver.md](./agents/build-error-resolver.md) |
| Review code | [agents.md](./rules/common/agents.md) → [code-reviewer.md](./agents/code-reviewer.md) |
| Write tests first (TDD) | [agents.md](./rules/common/agents.md) → [tdd-guide.md](./agents/tdd-guide.md) |
| Make architectural decisions | [agents.md](./rules/common/agents.md) → [architect.md](./agents/architect.md) |
| Understand commit format | [git-workflow.md](./rules/common/git-workflow.md) |
| Fix dark mode hydration | [mantine-dark-mode.md](./rules/common/mantine-dark-mode.md) |
| Manage database schema | [database-schema.md](./rules/common/database-schema.md) |
| Use notifications | [notifications.md](./rules/common/notifications.md) |
| Find the right skill/MCP server | [plugins.md](./rules/common/plugins.md) |
| Optimize performance | [performance.md](./rules/common/performance.md) |
| Configure permissions/hooks | [settings-hooks.md](./rules/common/settings-hooks.md) |

### Critical Files

**Read these first when:**

| Situation | File | Why |
|---|---|---|
| Hydration mismatch with dark mode | [mantine-dark-mode.md](./rules/common/mantine-dark-mode.md) | Critical SSR fix patterns |
| Starting any feature work | [agents.md](./rules/common/agents.md) | Understand agent workflow |
| Database schema errors | [database-schema.md](./rules/common/database-schema.md) | Better Auth + Drizzle integration |
| Before making a commit | [git-workflow.md](./rules/common/git-workflow.md) | Commit format enforcement |

## File Organization

```
.claude/
├── INDEX.md                      # This file - master documentation index
├── SCENARIOS.md                  # Task-based quick start guide
├── README.md                     # Directory overview and structure
├── settings.local.json           # Permissions + hooks configuration
├── agents/                       # Specialized sub-agents (7 files)
│   ├── architect.md              # System design (opus)
│   ├── planner.md                # Implementation planning (opus)
│   ├── build-error-resolver.md   # Fix build errors (sonnet)
│   ├── code-reviewer.md          # Code review (sonnet)
│   ├── refactor-cleaner.md       # Dead code cleanup (sonnet)
│   ├── security-reviewer.md      # Security analysis (sonnet)
│   └── tdd-guide.md              # Test-driven development (sonnet)
├── commands/                     # Slash command skills (1 file)
│   └── release.md                # /release skill workflow
└── rules/
    └── common/                   # Project rules (8 files)
        ├── agents.md             # Agent orchestration and workflow
        ├── database-schema.md    # Better Auth + Drizzle schema management
        ├── git-workflow.md       # Branching strategy, conventional commits
        ├── mantine-dark-mode.md  # SSR dark mode patterns (CRITICAL)
        ├── notifications.md      # Mantine notifications usage
        ├── performance.md        # Model selection, context management
        ├── plugins.md            # MCP servers, skills, decision tables
        ├── settings-hooks.md     # Configuration documentation
        └── versioning.md         # SemVer, release workflow
```

## Documentation Stats

- **Total files:** 18 documentation files
- **Agents:** 7 specialized agents (4 sonnet, 3 opus)
- **Rules:** 8 rule files (~1,200 lines)
- **Commands:** 1 command file
- **Lines of documentation:** ~1,600 lines total

## Contributing to Documentation

When adding or updating documentation:

1. **Add to this index** - Update the appropriate table above
2. **Add cross-references** - Link to related files using relative paths
3. **Use YAML frontmatter** - For agents and commands
4. **Mark scope** - Label rules as `general` or `project-specific`
5. **Update SCENARIOS.md** - If adding new workflows or patterns

## Related Files

- **Project-level:** [../CLAUDE.md](../CLAUDE.md) - Tech stack, architecture, commands
- **User-level:** `~/.claude/CLAUDE.md` - Personal coding preferences (not in repo)
- **Auto-memory:** `~/.claude/projects/<project>/memory/MEMORY.md` - Session learnings

## Getting Help

- `/help` - Get help with Claude Code features
- Issues: https://github.com/anthropics/claude-code/issues
- Project issues: https://github.com/InteLLigentMonk/Paddokk/issues
