---
name: warn-main-commits
enabled: true
event: bash
pattern: git\s+commit
action: warn
---

⚠️ **Git Commit Detected**

Before committing, verify you're **not on the `main` branch**.

**Why this matters:**
- All work should happen on feature branches (`feat/`, `fix/`, `refactor/`, etc.)
- Direct commits to `main` bypass the PR workflow
- Main should only receive squash merges from reviewed PRs

**Check your current branch:**
```bash
git branch --show-current
```

**If on main, create a feature branch:**
```bash
git checkout -b feat/your-feature-name
```

See `.claude/rules/common/git-workflow.md` for branching strategy.
