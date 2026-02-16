# Git Workflow with Conventional Commits & Semantic Versioning

## Quick Reference Card

| Action | Command |
|--------|---------|
| Start new feature | `git checkout -b feat/feature-name` |
| Start bug fix | `git checkout -b fix/bug-name` |
| Commit changes | `git commit -m "type(scope): description"` |
| Push branch | `git push -u origin branch-name` |
| Preview release | `npm run release:dry` |
| Create release | `npm run release` |
| Force major version | `npm run release:major` |
| Force minor version | `npm run release:minor` |
| Force patch version | `npm run release:patch` |
| Push release | `git push --follow-tags` |

**Commit Types:**
- `feat` - New feature (→ MINOR bump: 0.1.0 → 0.2.0)
- `fix` - Bug fix (→ PATCH bump: 0.1.0 → 0.1.1)
- `refactor` - Code refactoring (→ PATCH)
- `perf` - Performance improvement (→ PATCH)
- `chore` - Maintenance (→ hidden from changelog)
- `docs` - Documentation (→ hidden from changelog)
- `test` - Add/update tests (→ hidden from changelog)
- `feat!` or `BREAKING CHANGE:` - Breaking change (→ MAJOR bump: 1.0.0 → 2.0.0)

**Common Scopes:**
`auth`, `api`, `ui`, `journey`, `car`, `routing`, `query`, `form`, `db`, `landing`, `deps`, `release`

---

## Standard Development Workflow

### 1. Start New Work - Create a Feature Branch

```bash
# Make sure you're on main and up to date
git checkout main
git pull

# Create a new feature branch
git checkout -b feat/user-profile-page
# or: fix/login-button-alignment
# or: refactor/api-client
# or: chore/upgrade-dependencies
```

**Branch Naming Convention:**
- `feat/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `chore/` - Maintenance (deps, config)
- `docs/` - Documentation
- `test/` - Test additions
- `perf/` - Performance improvements

---

### 2. Do Your Work

Code normally - write your feature, fix the bug, refactor, etc.

Test your changes locally before committing.

---

### 3. Commit Changes - Use Conventional Commits

**Format:**
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Examples:**

```bash
# Simple feature
git commit -m "feat(auth): add password reset functionality"

# Bug fix
git commit -m "fix(ui): correct button alignment on mobile"

# With optional scope
git commit -m "feat(journey): add image upload to journey posts"

# With body for more context
git commit -m "feat(api): integrate user search endpoint

Added search functionality with filters for username, email, and join date.
Includes pagination and sorting options."

# Breaking change (use ! or BREAKING CHANGE footer)
git commit -m "feat(api)!: rename getUserProfile to getUser

BREAKING CHANGE: getUserProfile endpoint is now /api/users/get.
Update all API calls to use the new endpoint."

# Refactoring
git commit -m "refactor(query): extract API client to separate module"

# Performance improvement
git commit -m "perf(landing): lazy load hero images"

# Maintenance/chores
git commit -m "chore(deps): upgrade mantine to v8.4"
```

**The commitlint hook will validate your message** - if you write `"fixed the bug"` instead of `"fix: the bug"`, it will reject the commit.

---

### 4. Push Your Branch

```bash
# First push (sets upstream)
git push -u origin feat/user-profile-page

# Subsequent pushes
git push
```

**The pre-push hook runs tests automatically** - if tests fail, the push is blocked. Fix the tests before pushing.

---

### 5. Create a Pull Request

On GitHub:
1. Navigate to https://github.com/InteLLigentMonk/Paddokk
2. Click "Pull requests" → "New pull request"
3. Select your branch
4. Write a descriptive PR title (follows conventional format)
5. Fill in the PR description with:
   - Summary of changes
   - Test plan
   - Any breaking changes
6. Request review if working with a team

**PR Title should follow conventional format:**
```
feat(auth): add user profile page with avatar upload
fix(landing): resolve mobile menu toggle issue
refactor(api): improve error handling consistency
```

---

### 6. Merge to Main

After review and approval:
1. Use **Squash and Merge** (preferred method)
2. GitHub will combine all your commits into one
3. Ensure the squashed commit message follows conventional format
4. Click "Confirm squash and merge"
5. Delete the feature branch after merging

**Why squash merge?**
- Keeps main branch history clean
- One commit per feature/fix makes it easier to understand history
- Easier to revert if needed
- Better changelog generation

---

### 7. Create a Release (When Ready)

**Only create releases from the `main` branch:**

```bash
# 1. Switch to main and pull latest
git checkout main
git pull

# 2. Preview what will happen (dry run - always do this first!)
npm run release:dry

# 3. Review the output - check:
#    - Version bump is correct
#    - Changelog entries look good
#    - All commits are included

# 4. If everything looks good, create the release
npm run release

# 5. Push the release commit and tag
git push --follow-tags
```

**What `npm run release` does:**
1. ✅ Analyzes all commits since last tag
2. ✅ Determines version bump based on commit types
3. ✅ Updates `package.json` version field
4. ✅ Generates/updates `CHANGELOG.md` with organized commits
5. ✅ Creates a git commit: `chore(release): x.y.z`
6. ✅ Creates a git tag: `vx.y.z`

**Version Bump Rules:**

| Commit Type | Version Bump | Example |
|-------------|--------------|---------|
| `feat` | MINOR | 0.1.0 → 0.2.0 |
| `fix`, `perf`, `refactor` | PATCH | 0.1.0 → 0.1.1 |
| `feat!` or `BREAKING CHANGE:` | MAJOR | 1.0.0 → 2.0.0 |
| `chore`, `docs`, `test` | No release | (hidden from changelog) |

**Pre-1.0 (0.x.y) Convention:**
- Breaking changes → Bump MINOR (0.1.0 → 0.2.0)
- New features → Bump PATCH (0.1.0 → 0.1.1)
- Bug fixes → Bump PATCH (0.1.0 → 0.1.1)

**Force Specific Version:**
If the automatic detection isn't right, you can override:

```bash
npm run release:major   # 0.1.0 → 1.0.0
npm run release:minor   # 0.1.0 → 0.2.0
npm run release:patch   # 0.1.0 → 0.1.1
```

---

## Real-World Example Walkthrough

Let's say you want to add a "forgot password" feature:

```bash
# 1. Create branch from main
git checkout main
git pull
git checkout -b feat/forgot-password

# 2. Code your feature
# ... edit files, write tests ...

# 3. Commit with conventional format (can make multiple commits)
git add src/components/auth/forgot-password-form.tsx
git commit -m "feat(auth): add forgot password form component"

git add src/routes/_auth/forgot-password.tsx
git commit -m "feat(auth): add forgot password route"

git add src/lib/api/auth.ts
git commit -m "feat(api): add password reset token endpoint"

# 4. Push branch (pre-push hook runs tests)
git push -u origin feat/forgot-password

# 5. Create PR on GitHub
# Title: "feat(auth): add forgot password flow"
# Description: Explain what was added

# 6. After approval, squash merge to main
# GitHub combines all commits into one

# 7. When ready to release (could be immediately or later):
git checkout main
git pull
npm run release:dry    # Preview: 0.1.0 → 0.2.0 (MINOR bump due to feat)
npm run release        # Create release
git push --follow-tags # Push to remote

# Done! Version 0.2.0 is now released with updated CHANGELOG
```

---

## Commit Message Tips

### ✅ Good Examples

```bash
git commit -m "feat(auth): add email verification flow"
git commit -m "fix(ui): prevent double-tap on submit button"
git commit -m "perf(query): add caching to user profile queries"
git commit -m "refactor(api): extract validation logic to separate files"
git commit -m "chore(deps): upgrade tanstack router to v1.132"
git commit -m "docs(readme): add API setup instructions"
git commit -m "test(auth): add unit tests for login validation"
```

### ❌ Bad Examples (Will be rejected by commitlint)

```bash
git commit -m "fixed bug"              # Missing type format
git commit -m "updates"                # Too vague, no type
git commit -m "WIP"                    # Not descriptive
git commit -m "feat: stuff"            # Too vague
git commit -m "add password reset"     # Missing type prefix
```

### Writing Good Commit Messages

**The subject line (first line) should:**
- Start with type (and optional scope)
- Use imperative mood ("add" not "added" or "adds")
- Not end with a period
- Be 100 characters or less
- Describe what the commit does, not what you did

**Optional body should:**
- Explain WHY, not WHAT (the diff shows what)
- Wrap at 72 characters
- Separate from subject with blank line

**Example with body:**
```bash
git commit -m "feat(journey): add image compression before upload

Images are now compressed to max 1920px width and 85% quality
before upload to reduce storage costs and improve load times.
Uses browser Canvas API with fallback for older browsers."
```

---

## Common Scenarios

### Scenario 1: Quick Bug Fix

```bash
git checkout main
git pull
git checkout -b fix/broken-logout
# ... fix the bug ...
git commit -m "fix(auth): clear session data on logout"
git push -u origin fix/broken-logout
# Create PR, merge, done!
```

### Scenario 2: Multiple Related Changes

```bash
git checkout -b feat/user-dashboard
# ... work on feature ...
git commit -m "feat(ui): add dashboard layout component"
# ... more work ...
git commit -m "feat(dashboard): add recent activity widget"
# ... more work ...
git commit -m "feat(dashboard): add stats summary cards"
git push -u origin feat/user-dashboard
# PR will squash all commits into one on merge
```

### Scenario 3: Breaking API Change

```bash
git checkout -b feat/api-v2
# ... make breaking changes ...
git commit -m "feat(api)!: restructure user endpoints

BREAKING CHANGE: User endpoints now follow REST conventions:
- GET /api/users/:id (was /api/getUserProfile)
- PUT /api/users/:id (was /api/updateUserProfile)
- DELETE /api/users/:id (was /api/deleteUser)

Update all API calls to use new endpoint structure."
git push -u origin feat/api-v2
# This will trigger MAJOR version bump when released
```

### Scenario 4: Dependency Updates

```bash
git checkout -b chore/update-mantine
npm update @mantine/core @mantine/hooks
git commit -m "chore(deps): update mantine to v8.4"
git push -u origin chore/update-mantine
# Chore commits don't appear in changelog
```

---

## Troubleshooting

### Problem: Commit message rejected by hook

**Error:**
```
✖   type may not be empty [type-empty]
✖   subject may not be empty [subject-empty]
```

**Solution:**
Use the correct format: `type(scope): description`

```bash
# Wrong
git commit -m "added new feature"

# Right
git commit -m "feat(ui): add new feature"
```

### Problem: Pre-push hook blocks push (tests failing)

**Error:**
```
✖   Tests failed
husky - pre-push hook failed (code 1)
```

**Solution:**
Fix the failing tests before pushing.

```bash
npm test              # Run tests locally
# Fix the failing tests
git add .
git commit -m "test(ui): fix failing button test"
git push              # Try again
```

### Problem: Wrong version bump after release

**Issue:**
Expected minor bump (0.1.0 → 0.2.0) but got patch (0.1.0 → 0.1.1)

**Cause:**
No `feat` commits since last release, only `fix` commits.

**Solution:**
Use manual override if needed:

```bash
npm run release:minor  # Force minor bump
```

### Problem: Forgot to push tags

**Issue:**
Created release locally but GitHub doesn't show the tag.

**Solution:**
```bash
git push --follow-tags
```

---

## Best Practices

1. **Commit Often**
   - Small, focused commits are easier to review
   - Each commit should be a logical unit of change
   - One feature/fix per commit when possible

2. **Use Descriptive Scopes**
   - `feat(auth): add login` is better than `feat: add login`
   - Helps organize changelog by feature area
   - Makes it easier to find related changes

3. **Test Before Committing**
   - Run `npm test` locally
   - Ensure code works as expected
   - Hooks will catch issues, but save time by testing first

4. **Preview Releases**
   - Always run `npm run release:dry` first
   - Review changelog entries
   - Verify version bump is correct

5. **Write for Future You**
   - 6 months from now, will you understand this commit?
   - Include context in the body if needed
   - Link to issues/tickets if relevant

6. **Keep Main Clean**
   - Never commit directly to main (except emergency hotfixes)
   - Always work on feature branches
   - Use squash merge to keep history clean

7. **Release When Ready**
   - Don't feel pressured to release after every merge
   - Group related features into releases
   - Communicate breaking changes clearly

---

## Resources

- **Conventional Commits:** https://www.conventionalcommits.org/
- **Semantic Versioning:** https://semver.org/
- **commit-and-tag-version:** https://github.com/absolute-version/commit-and-tag-version
- **Commitlint:** https://commitlint.js.org/

---

## Need Help?

- View commit history: `git log --oneline`
- View changelog: `cat client/CHANGELOG.md`
- View git tags: `git tag -l`
- View release commits: `git log --oneline --grep="chore(release)"`
- Test commit message format: Just try to commit - the hook will validate!

**Remember:** The hooks are there to help, not hinder. They ensure consistency and make the release process automatic. Once you get used to the format, it becomes second nature!
