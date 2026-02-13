---
scope: general
applies-to: [permissions, hooks, automation]
read-when: [configuring-permissions, setting-up-hooks, recreating-config]
---

# Settings & Hooks Configuration

Documentation for `settings.local.json` - permissions and hooks that automate Claude Code workflows.

## File Location

`client/.claude/settings.local.json`

## Structure

```json
{
  "permissions": {
    "allow": [/* auto-allowed tool patterns */]
  },
  "hooks": {
    "PostToolUse": [/* automation after tool use */]
  }
}
```

## Permissions

Auto-allowed tool patterns that don't require user confirmation.

### Current Auto-Allowed Commands

| Pattern | Purpose | Examples |
|---|---|---|
| `Bash(npm run *)` | Package scripts | `npm run dev`, `npm run build`, `npm run test` |
| `Bash(npx vitest *)` | Test execution | `npx vitest run`, `npx vitest watch` |
| `Bash(npx prettier *)` | Code formatting | `npx prettier --write .` |
| `Bash(npx eslint *)` | Linting | `npx eslint --fix` |
| `Bash(git status)` | Git status | View working tree status |
| `Bash(git log *)` | Git history | `git log --oneline`, `git log -p` |
| `Bash(git diff *)` | Git diff | `git diff`, `git diff --staged` |
| `Bash(git branch *)` | Branch operations | `git branch`, `git branch -a` |
| `Bash(git add:*)` | Stage changes | `git add .`, `git add file.ts` |
| `Bash(git checkout:*)` | Switch branches | `git checkout main`, `git checkout -b feat/new` |
| `Bash(npm install:*)` | Install dependencies | `npm install`, `npm install package` |
| `Bash(npx @better-auth/cli generate:*)` | Better Auth CLI | Generate auth schema |
| `Bash(npx drizzle-kit push:*)` | Drizzle push | Sync schema to database |
| `Bash(npx tsc:*)` | TypeScript compiler | `npx tsc --noEmit` |
| `Bash(ls *)` | List files | `ls -la`, `ls src/` |
| `Bash(echo *)` | Echo output | `echo "message"` |
| `Bash(cat *)` | Read files | `cat package.json` |
| `Bash(wc:*)` | Word count | `wc -l file.ts` |
| `Bash(netstat:*)` | Network stats | `netstat -ano` (Windows) |
| `Bash(findstr:*)` | Find string | `findstr "pattern" file` (Windows) |
| `Bash(taskkill:*)` | Kill process | `taskkill /PID 1234` (Windows) |

### MCP Server Permissions

| Pattern | Purpose |
|---|---|
| `mcp__plugin_context7_context7__resolve-library-id` | Resolve library IDs for Context7 |
| `mcp__plugin_context7_context7__query-docs` | Query library documentation |
| `mcp__plugin_github_github__*` | All GitHub MCP operations (issues, PRs, code search) |

### Adding New Permissions

Edit `settings.local.json` and add patterns to the `permissions.allow` array:

```json
{
  "permissions": {
    "allow": [
      "Bash(your-command-pattern *)"
    ]
  }
}
```

**Pattern syntax:**
- `*` - Wildcard for any arguments
- `:*` - Separator for Windows-style commands
- Exact matches for specific commands

## Hooks

Automated actions triggered by tool usage events.

### PostToolUse Hook: Auto-Format with Prettier

**Configuration:**
```json
{
  "hooks": {
    "PostToolUse": [
      {
        "matcher": "Edit|Write",
        "hooks": [
          {
            "type": "command",
            "command": "jq -r '.tool_input.file_path // empty' | xargs npx prettier --write 2>/dev/null || true",
            "timeout": 30
          }
        ]
      }
    ]
  }
}
```

**How it works:**
1. **Trigger:** Fires after any `Edit` or `Write` tool use
2. **Extract file path:** Uses `jq` to extract `file_path` from tool input JSON
3. **Format:** Runs `npx prettier --write <file_path>`
4. **Timeout:** 30 seconds max
5. **Silent:** `2>/dev/null || true` suppresses errors (won't fail if Prettier errors)

**Effect:** Every file you edit or create is automatically formatted by Prettier according to project rules.

### Hook Events

Available event types for hooks:

| Event | When it fires |
|---|---|
| `PreToolUse` | Before a tool is executed |
| `PostToolUse` | After a tool is executed |
| `PreTurn` | Before Claude generates a response |
| `PostTurn` | After Claude generates a response |

### Hook Matchers

For `PreToolUse` and `PostToolUse` events, the `matcher` field filters which tools trigger the hook:

- `Edit` - Only Edit tool
- `Write` - Only Write tool
- `Edit|Write` - Either Edit or Write
- `Bash` - Only Bash tool
- `Read|Grep|Glob` - Any of these tools

### Hook Types

| Type | Purpose | Example |
|---|---|
| `command` | Run a shell command | Auto-format, run linter |
| `message` | Display a message to user | Reminders, warnings |
| `block` | Block the tool use | Prevent dangerous commands |

### Example: Branch Protection Hook

**Goal:** Warn before committing to `main` branch.

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Bash",
        "hooks": [
          {
            "type": "message",
            "message": "⚠️ Check you're not on main branch before committing!",
            "command": "git commit"
          }
        ]
      }
    ]
  }
}
```

### Example: Force Push Block

**Goal:** Block `git push --force` and suggest `--force-with-lease`.

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Bash",
        "hooks": [
          {
            "type": "block",
            "message": "Blocked: Use --force-with-lease instead of --force to prevent overwriting others' work",
            "command": "git push --force"
          }
        ]
      }
    ]
  }
}
```

## Recreating Configuration with /hookify

The `/hookify` skill provides an interactive way to create hooks from conversation analysis or explicit instructions.

**Usage:**
```
/hookify
```

**What it does:**
1. Analyzes conversation for unwanted behaviors or patterns
2. Suggests hook rules to prevent them
3. Creates or updates hooks in `settings.local.json`

**Use cases:**
- Convert manual reminders into automated hooks
- Block commands you keep running by accident
- Add warnings for risky operations
- Automate repetitive post-action tasks

**Related commands:**
- `/hookify configure` - Enable/disable rules interactively
- `/hookify list` - List all configured rules
- `/hookify help` - Get help with hookify

## Recommended Hooks

Document these in your project so developers can recreate them:

### 1. Auto-Format Hook (Current)

**Purpose:** Format code automatically after editing or writing files.

**Benefit:** Consistent code style without manual formatting.

### 2. Branch Protection Hook

**Purpose:** Warn before committing to `main` branch.

**Benefit:** Prevents accidental commits to production branch.

### 3. Force Push Protection Hook

**Purpose:** Block `git push --force`, suggest `--force-with-lease`.

**Benefit:** Prevents overwriting others' work.

## Best Practices

### Permissions

- **Start restrictive:** Only allow commands you use frequently
- **Add as needed:** Add new patterns when you find yourself approving the same command repeatedly
- **Use wildcards carefully:** `Bash(git *)` allows ALL git commands, which may be too permissive
- **Separate by concern:** Group related commands together in the JSON for readability

### Hooks

- **Use PostToolUse for automation:** Auto-format, auto-test, auto-lint
- **Use PreToolUse for safety:** Warnings, blocks, confirmations
- **Silent failures for automation:** Use `|| true` for hooks that shouldn't block on errors
- **Timeouts:** Set reasonable timeouts (30s for formatting, 60s for tests)
- **Test hooks:** Verify hooks work as expected before relying on them

## Troubleshooting

### Hook Not Firing

**Possible causes:**
1. **Matcher doesn't match:** Check tool name matches exactly (case-sensitive)
2. **JSON syntax error:** Validate JSON with `npx prettier settings.local.json`
3. **Command pattern wrong:** Test the command manually first
4. **File not loaded:** Restart Claude Code to reload settings

### Permission Not Working

**Possible causes:**
1. **Pattern doesn't match:** Use more specific pattern or add wildcards
2. **Typo in pattern:** Check exact command format
3. **Not in allow array:** Ensure pattern is in `permissions.allow`

### Hook Command Fails

**Possible causes:**
1. **Command not found:** Ensure command is in PATH
2. **Timeout too short:** Increase timeout value
3. **Syntax error:** Test command in terminal first
4. **Silent failure:** Check if `2>/dev/null || true` is hiding real errors

## Configuration File Reference

Full example `settings.local.json`:

```json
{
  "permissions": {
    "allow": [
      "Bash(npm run *)",
      "Bash(npx vitest *)",
      "Bash(npx prettier *)",
      "Bash(npx eslint *)",
      "Bash(git status)",
      "Bash(git log *)",
      "Bash(git diff *)",
      "Bash(git branch *)",
      "Bash(git add:*)",
      "Bash(git commit -m *)",
      "Bash(git checkout:*)",
      "mcp__plugin_context7_context7__resolve-library-id",
      "mcp__plugin_context7_context7__query-docs",
      "mcp__plugin_github_github__*"
    ]
  },
  "hooks": {
    "PostToolUse": [
      {
        "matcher": "Edit|Write",
        "hooks": [
          {
            "type": "command",
            "command": "jq -r '.tool_input.file_path // empty' | xargs npx prettier --write 2>/dev/null || true",
            "timeout": 30
          }
        ]
      }
    ],
    "PreToolUse": [
      {
        "matcher": "Bash",
        "hooks": [
          {
            "type": "message",
            "message": "Check current branch before committing!",
            "pattern": "git commit"
          }
        ]
      }
    ]
  }
}
```

## Related Documentation

- See [git-workflow.md](./git-workflow.md) for recommended branch protection and force push hooks
- See [plugins.md](./plugins.md) for `/hookify` skill usage
- See [README.md](../../README.md) for overview of current setup

## Further Reading

- Claude Code hooks documentation: https://docs.claude.ai/claude-code/hooks
- JSON pattern matching: https://jqlang.github.io/jq/
