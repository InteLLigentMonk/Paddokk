---
scope: general
applies-to: [git, branching, commits, conventional-commits, pr-workflow]
read-when: [making-commits, creating-pr, branching-strategy]
---

# Git Workflow

## Branching Strategy

- **Main branch:** `main` is the production branch. All releases are tagged here.
- **Feature branches:** All work happens on `<type>/<description>` branches:
  - `feat/user-profile` - New features
  - `fix/auth-redirect` - Bug fixes
  - `refactor/query-cache` - Code refactoring
  - `chore/upgrade-deps` - Maintenance work
  - `docs/api-guide` - Documentation
  - `test/journey-flows` - Test additions
- **No develop branch:** Work directly off `main`, merge via PRs
- **No release branches:** Tags on `main` mark releases

## Branch Creation

**CRITICAL:** Always check the current branch before making changes. If on `main`, create an appropriate feature branch first.

```bash
git checkout -b feat/new-feature  # Create feature branch from main
```

Never commit directly to `main` unless it's an emergency hotfix.

## Commit Format

Follow [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types

| Type       | Purpose                                      | Changelog Section |
| ---------- | -------------------------------------------- | ----------------- |
| `feat`     | New feature                                  | Features          |
| `fix`      | Bug fix                                      | Bug Fixes         |
| `perf`     | Performance improvement                      | Performance       |
| `refactor` | Code change that neither fixes nor adds      | Refactoring       |
| `chore`    | Maintenance (deps, config, tooling)          | Maintenance       |
| `revert`   | Revert a previous commit                     | Reverts           |
| `docs`     | Documentation only                           | (hidden)          |
| `style`    | Formatting, whitespace, semicolons           | (hidden)          |
| `test`     | Add/update tests                             | (hidden)          |
| `ci`       | CI/CD configuration                          | (hidden)          |

### Scopes

Common scopes (warning-level enforcement, new scopes allowed):

- `auth` - Authentication/authorization
- `api` - API integration (Orval, axios)
- `ui` - UI components
- `journey` - Journey feature
- `car` - Car feature
- `routing` - TanStack Router
- `query` - TanStack Query
- `form` - TanStack Form
- `db` - Database schema/queries
- `landing` - Landing page
- `deps` - Dependencies
- `release` - Release process

### Breaking Changes

Mark breaking changes with `!` suffix or `BREAKING CHANGE:` footer:

```
feat(api)!: rename getUserProfile to getUser

BREAKING CHANGE: getUserProfile is now getUser. Update all call sites.
```

### Examples

```
feat(auth): add password reset flow
fix(journey): prevent duplicate image uploads
perf(query): add staleTime to user queries
refactor(ui): extract form validation to hook
chore(deps): upgrade mantine to v8.4
docs(readme): add API generation steps
test(auth): add login flow E2E tests
```

## PR Workflow

1. **Create PR:** Use descriptive title following conventional commit format
2. **Review:** Get code review before merging
3. **Merge:** Use **squash merge** to `main` (keeps history clean)
4. **Delete branch:** Delete feature branch after merge

PR title becomes the commit message on `main`, so make it count.

## Recommended Claude Code Hooks

Document these hooks so developers can recreate them with `/hookify`:

### Branch Protection Hook

- **Event:** PreToolUse on Bash
- **Matcher:** commands containing `git commit`
- **Action:** Warn to check current branch is not `main`
- **Purpose:** Prevent accidental commits to main

### Force Push Protection Hook

- **Event:** PreToolUse on Bash
- **Matcher:** commands containing `git push --force` or `git push -f`
- **Action:** Block with message to use `--force-with-lease` instead
- **Purpose:** Prevent overwriting others' work

To create these hooks, run `/hookify` and follow the prompts.

## Related Documentation

- [versioning.md](./versioning.md) - SemVer rules, release workflow, version bumping
- [agents.md](./agents.md) - Feature implementation workflow
- [commands/release.md](../../commands/release.md) - `/release` skill workflow
- [plugins.md](./plugins.md) - `/commit` and `/commit-push-pr` skills
- [settings-hooks.md](./settings-hooks.md) - Recommended branch protection and force push hooks

**Config files:**
- `commitlint.config.js` - Commit message linting rules
- `.versionrc.json` - Changelog generation config
