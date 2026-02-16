---
name: update-docs
description: Synchronize documentation with code changes using the docs-updater agent
scope: general
read-when: [updating-docs, pre-commit, syncing-documentation]
---

You are helping the user synchronize documentation with code changes.

## What This Does

Launches the **docs-updater agent** to analyze staged changes and update CLAUDE.md and README.md as needed.

## Workflow

### 1. Check for Staged Changes

```bash
git diff --cached --name-status
```

If nothing is staged:

```
ℹ️  No staged changes found.

Stage your changes first:
  git add <files>

Then run /update-docs again.
```

Exit if nothing is staged.

### 2. Launch docs-updater Agent

If changes are staged, immediately launch the agent:

```
Launching docs-updater agent to sync documentation...
```

Use the **Task tool** with `subagent_type: general` and agent specification:

```
Use the docs-updater agent to analyze staged changes and update documentation.
```

The docs-updater agent will:
- Analyze `git diff --cached` to see what changed
- Determine if CLAUDE.md or README.md need updates
- Make targeted documentation updates
- Report what was changed

### 3. Done

The agent handles everything. No additional steps needed in this command.

## Related Documentation

- [../agents/docs-updater.md](../agents/docs-updater.md) - Documentation agent details
- [../rules/common/settings-hooks.md](../rules/common/settings-hooks.md) - Pre-commit hook configuration
- [../rules/common/git-workflow.md](../rules/common/git-workflow.md) - Git workflow
