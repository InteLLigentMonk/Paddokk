# Claude Code Configuration

This directory contains Claude Code configuration for the Paddokk client project.

## Structure

```
.claude/
├── agents/                 # Specialized sub-agents
│   ├── architect.md        # System design (opus)
│   ├── planner.md          # Implementation planning (opus)
│   ├── code-reviewer.md    # Code review (sonnet)
│   ├── tdd-guide.md        # Test-driven development (sonnet)
│   ├── build-error-resolver.md  # Fix build errors (sonnet)
│   ├── security-reviewer.md     # Security analysis (sonnet)
│   └── refactor-cleaner.md      # Dead code cleanup (sonnet)
├── rules/
│   └── common/
│       ├── agents.md       # Agent orchestration and workflow
│       └── performance.md  # Model selection, context management
├── settings.local.json     # Permissions + hooks config
└── README.md               # This file
```

## Agents

7 specialized agents with right-sized models. See `rules/common/agents.md` for the full table and usage guidance.

## Rules

2 rule files loaded into every conversation:

- **agents.md** - Agent selection, parallel execution, feature implementation workflow
- **performance.md** - Model selection strategy, context window management, extended thinking

All other guidelines live in the project `CLAUDE.md` (always loaded) and the user-level `~/.claude/CLAUDE.md`.

## Hooks

Auto-formatting via PostToolUse hook in `settings.local.json`: runs Prettier on any file after Edit/Write.

## Permissions

`settings.local.json` defines auto-allowed commands for npm, vitest, prettier, eslint, git, and MCP plugins.

## Auto-Memory

Claude Code's auto-memory lives at `~/.claude/projects/<project>/memory/MEMORY.md` (not in this directory). It persists learnings across conversations automatically.
