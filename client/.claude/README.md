# Claude Code Configuration

This directory contains Claude Code configuration for the Paddokk client project.

## Quick Navigation

- [INDEX.md](./INDEX.md) - Complete documentation index with all 18 files
- [SCENARIOS.md](./SCENARIOS.md) - Task-based quick start guide ("I want to do X, what should I read?")
- [CLAUDE.md](../CLAUDE.md) - Project-level configuration (tech stack, architecture)

## Structure

```
.claude/
├── INDEX.md                      # Master documentation index
├── SCENARIOS.md                  # Task-based quick start guide
├── README.md                     # This file
├── settings.local.json           # Permissions + hooks config
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

## Agents

7 specialized agents with right-sized models. See `rules/common/agents.md` for the full table and usage guidance.

## Rules

8 rule files loaded into every conversation:

- **agents.md** - Agent selection, parallel execution, feature implementation workflow
- **database-schema.md** - Better Auth + Drizzle schema management, push vs migrate workflow
- **git-workflow.md** - Branching strategy, conventional commits, PR workflow
- **mantine-dark-mode.md** - SSR dark mode patterns, hydration fix (CRITICAL)
- **notifications.md** - Mantine notifications usage patterns, automatic error handling
- **performance.md** - Model selection strategy, context window management, extended thinking
- **plugins.md** - MCP servers (Context7, GitHub), skills, decision tables
- **settings-hooks.md** - Permissions and hooks configuration documentation
- **versioning.md** - SemVer rules, release workflow, changelog generation

All other guidelines live in the project `CLAUDE.md` (always loaded) and the user-level `~/.claude/CLAUDE.md`.

## Hooks

Auto-formatting via PostToolUse hook in `settings.local.json`: runs Prettier on any file after Edit/Write.

See [rules/common/settings-hooks.md](./rules/common/settings-hooks.md) for full hook documentation and how to recreate configuration with `/hookify`.

## Permissions

`settings.local.json` defines auto-allowed commands for npm, vitest, prettier, eslint, git, and MCP plugins.

See [rules/common/settings-hooks.md](./rules/common/settings-hooks.md) for complete permissions list and how to add new patterns.

## Auto-Memory

Claude Code's auto-memory lives at `~/.claude/projects/<project>/memory/MEMORY.md` (not in this directory). It persists learnings across conversations automatically.
