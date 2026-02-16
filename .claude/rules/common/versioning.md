---
scope: general
applies-to: [semver, releases, versioning, changelog]
read-when: [creating-release, understanding-semver, version-bumping]
---

# Versioning & Releases

## Semantic Versioning

Paddokk follows [Semantic Versioning](https://semver.org/) (SemVer): `MAJOR.MINOR.PATCH`

### Version Components

| Component | When to Bump                                             | Example          |
| --------- | -------------------------------------------------------- | ---------------- |
| MAJOR     | Breaking changes (incompatible API changes)              | `1.0.0` → `2.0.0` |
| MINOR     | New features (backwards-compatible functionality)        | `1.0.0` → `1.1.0` |
| PATCH     | Bug fixes (backwards-compatible fixes)                   | `1.0.0` → `1.0.1` |

### Pre-1.0 Convention

While in early development (`0.x.y`), the rules differ:

- **Breaking changes** → Bump MINOR (`0.1.0` → `0.2.0`)
- **New features** → Bump PATCH (`0.1.0` → `0.1.1`)
- **Bug fixes** → Bump PATCH (`0.1.0` → `0.1.1`)

Version `1.0.0` signals production readiness and stable API.

## Automatic Version Bumping

Version bumps are determined by commit types since the last release:

| Commit Types Present        | Bump    | Example          |
| --------------------------- | ------- | ---------------- |
| `feat!` or `BREAKING CHANGE:` | MAJOR   | `0.1.0` → `1.0.0` (if >= 1.0) or `0.1.0` → `0.2.0` (if < 1.0) |
| `feat`                      | MINOR   | `0.1.0` → `0.2.0` (if < 1.0) or `1.0.0` → `1.1.0` (if >= 1.0) |
| `fix`, `perf`, `refactor`   | PATCH   | `0.1.0` → `0.1.1` |

The `commit-and-tag-version` tool parses commits and determines the bump automatically.

## Release Commands

| Command                  | Purpose                                |
| ------------------------ | -------------------------------------- |
| `npm run release`        | Auto-bump based on commits             |
| `npm run release:dry`    | Preview version bump (no changes)      |
| `npm run release:first`  | First release (creates tag, no bump)   |
| `npm run release:major`  | Force MAJOR bump                       |
| `npm run release:minor`  | Force MINOR bump                       |
| `npm run release:patch`  | Force PATCH bump                       |

Use `release:dry` to preview before running the real release.

## Release Workflow

Use the `/release` skill for guided workflow (see [commands/release.md](../../commands/release.md) for detailed steps).

### Manual Release

1. **Ensure on `main` branch:**
   ```bash
   git checkout main
   git pull
   ```

2. **Preview the release:**
   ```bash
   npm run release:dry
   ```

3. **Run the release:**
   ```bash
   npm run release
   ```

   This will:
   - Bump version in `package.json`
   - Generate/update `CHANGELOG.md`
   - Create a git commit with the new version
   - Create a git tag (e.g., `v0.2.0`)

4. **Push to remote with tags:**
   ```bash
   git push --follow-tags
   ```

## What Gets Generated

### CHANGELOG.md

Automatically generated from conventional commits. Organized by:

- **Features** (`feat`)
- **Bug Fixes** (`fix`)
- **Performance** (`perf`)
- **Refactoring** (`refactor`)
- **Maintenance** (`chore`)
- **Reverts** (`revert`)

Commits with `docs`, `style`, `test`, `ci` are hidden from changelog.

### Git Tag

Format: `v<version>` (e.g., `v0.2.0`, `v1.0.0`)

Tags are annotated and include the changelog for that version.

## Configuration Files

- `client/package.json` - `version` field, release scripts
- `client/.versionrc.json` - Changelog sections, URL formats, tag prefix
- See [git-workflow.md](./git-workflow.md) for commit message format

## First Release

For the very first release, use:

```bash
npm run release:first
```

This creates the tag and CHANGELOG without bumping (since there's no previous version).

## Troubleshooting

### "No commits since last release"

If you run `npm run release` but there are no new commits since the last tag, the tool will error. Ensure you have commits to release.

### Wrong version bump

Use the explicit `release:major`, `release:minor`, or `release:patch` commands to override the automatic detection.

### Forgot to push tags

Tags are not pushed by default. Always use:

```bash
git push --follow-tags
```

Or use the `/release` skill which handles this automatically.

## Related Documentation

- [commands/release.md](../../commands/release.md) - Detailed `/release` skill workflow
- [git-workflow.md](./git-workflow.md) - Commit format and branching strategy
- [plugins.md](./plugins.md) - Skills and MCP servers
