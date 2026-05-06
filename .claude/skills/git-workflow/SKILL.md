---
name: git-workflow
description: Git workflow Skill. ALWAYS invoke this skill before starting any new feature, fix, chore, documentation work, refactor or task. Do not attempt to implement anything directly - use this skill first.
model: haiku
triggers:
  - TRIGGER AUTOMATICALLY before starting any new feature, fix, or task — always checkout main and pull before branching
  - TRIGGER AUTOMATICALLY before any commit or push operation
  - TRIGGER AUTOMATICALLY before opening a PR
  - DO NOT skip this skill and begin implementation directly — branch setup always comes first
---

# Git Workflow

GitHub Flow with conventional commits and automated semantic versioning.

## Branching

- `main` is the production branch — always deployable
- All work happens on feature branches off `main`
- Branch naming: `feat/short-description`, `fix/short-description`, `chore/short-description`
- Keep branches short-lived — one feature or fix per branch
- Always merge via PR, never push directly to main

## Conventional Commits

Every commit message follows the format:

```
<type>(<optional scope>): <description>

[optional body]

[optional footer]
```

### Types
- `feat`: New feature (triggers MINOR version bump)
- `fix`: Bug fix (triggers PATCH version bump)
- `docs`: Documentation only
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `test`: Adding or updating tests
- `chore`: Build process, dependencies, tooling
- `perf`: Performance improvement
- `ci`: CI/CD changes

### Breaking Changes
Add `!` after the type or include `BREAKING CHANGE:` in the footer. Triggers MAJOR version bump.

```
feat!: replace car search API with new filter system

BREAKING CHANGE: /api/cars/search endpoint removed, use /api/cars/filter instead
```

### Examples
```
feat(cars): add make/model/generation hierarchy
fix(auth): handle expired BetterAuth refresh tokens
refactor(bff): extract shared server function helpers
test(cars): add unit tests for CreateCarCommandHandler
chore: update EF Core to latest version
docs: add API documentation for car endpoints
ci: add PostgreSQL service to GitHub Actions workflow
```

### Rules
- Use imperative mood: "add feature" not "added feature" or "adds feature"
- Lowercase first letter after the colon
- No period at the end of the description
- Scope is optional but encouraged — use the feature name (cars, auth, users, etc.)
- Keep description under 72 characters
- Use the body for "what and why" if the description isn't enough
- Dont append Generated with Claude Code or similar to the commit message — the commit history should be clean and focused on the changes, not the tooling

## Pull Requests

### Before Opening a PR
- Rebase on latest main: `git fetch origin && git rebase origin/main`
- Ensure all checks pass locally: `dotnet build`, `dotnet test`, `pnpm build`
- Squash WIP commits into meaningful conventional commits if needed

### PR Title
Use conventional commit format for the PR title — this is what the squash merge commit message becomes.

```
feat(cars): add car database CRUD endpoints
```

### PR Description
- Brief summary of what changed and why
- List any breaking changes
- Note if migrations need to be run
- Link to related issues if any

## Versioning

Automated via conventional commits (release-please or similar):
- `fix:` → PATCH (1.0.0 → 1.0.1)
- `feat:` → MINOR (1.0.0 → 1.1.0)
- `BREAKING CHANGE` or `!` → MAJOR (1.0.0 → 2.0.0)
- `docs:`, `chore:`, `test:`, `refactor:`, `ci:` → no version bump

## Common Workflows

### Starting a new feature
```bash
git checkout main
git pull origin main
git checkout -b feat/car-database
```

### Daily work
```bash
# Work, then commit with conventional format
git add .
git commit -m "feat(cars): add Car entity and EF configuration"

# Stay up to date with main
git fetch origin
git rebase origin/main
```

### Ready to merge
```bash
git push origin feat/car-database

# Create PR via GitHub CLI
gh pr create --title "feat(cars): add car database CRUD endpoints" --body "Summary of changes" --base main

# Or interactively (gh will prompt for title, body, reviewers)
gh pr create
```

### Useful gh commands
```bash
gh pr list                          # List open PRs
gh pr view                          # View current branch's PR
gh pr checks                        # Check CI status on current PR
gh pr merge --squash --delete-branch # Squash merge and clean up
```

### After merge
```bash
git checkout main
git pull origin main
git branch -d feat/car-database
```