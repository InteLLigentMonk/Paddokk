---
name: block-force-push
enabled: true
event: bash
pattern: git\s+push\s+(--force|-f)(\s|$)
action: block
---

🚫 **Force Push Blocked**

Use `--force-with-lease` instead of `--force` or `-f`.

**Why this matters:**
- `git push --force` can overwrite others' work
- `--force-with-lease` checks if the remote has changed since your last fetch
- Prevents accidentally destroying teammates' commits

**Safe alternative:**
```bash
git push --force-with-lease
```

This will fail if someone else has pushed to the branch, protecting their work.

**Only use --force-with-lease when:**
- You've rebased your own feature branch
- You're fixing commit history on a branch you own
- You've confirmed with the team that force-pushing is safe

See `.claude/rules/common/git-workflow.md` for git workflow guidelines.
