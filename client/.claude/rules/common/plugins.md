---
scope: general
applies-to: [mcp-servers, skills, context7, github, tools]
read-when: [finding-right-tool, using-context7, github-operations]
---

# Plugins, Skills & MCP Servers

## MCP Servers

### Context7 (`mcp__plugin_context7_context7`)

Up-to-date documentation and code examples for any library.

**When to use:**

- Looking up API usage for project dependencies (Mantine v8, TanStack Router/Query/Form/Store, Tiptap v3, Better Auth, Zod v4)
- Unsure about a library's current API or patterns
- Checking for breaking changes or migration guides
- Before implementing features that depend on third-party library APIs

**How to use:**

1. Call `resolve-library-id` first to get the library ID
2. Call `query-docs` with the library ID and a specific question

**Proactive usage:** Always consult Context7 before writing code that uses a library API you're not 100% certain about. Prefer verified docs over guessing.

### GitHub MCP (`mcp__plugin_github_github`)

Full GitHub API access for issues, PRs, branches, code search, and repository management.

**When to use:**

- Creating/reading/updating issues and PRs
- Searching code across repositories
- Managing branches and releases
- Reviewing PR diffs and comments

## Skills (Slash Commands)

### Development Workflow

| Skill           | Command            | Purpose                                                                  |
| --------------- | ------------------ | ------------------------------------------------------------------------ |
| feature-dev     | `/feature-dev`     | Guided feature development with codebase analysis and architecture focus |
| frontend-design | `/frontend-design` | Production-grade UI components with high design quality                  |
| ralph-loop      | `/ralph-loop`      | Iterative development loop for sustained coding sessions                 |

### Git & PR Workflow

| Skill          | Command           | Purpose                                          | Documentation |
| -------------- | ----------------- | ------------------------------------------------ | ------------- |
| commit         | `/commit`         | Create a git commit                              | See [git-workflow.md](./git-workflow.md) |
| commit-push-pr | `/commit-push-pr` | Commit, push, and open a PR in one step          | See [git-workflow.md](./git-workflow.md) |
| release        | `/release`        | Guided version bump and changelog generation     | [commands/release.md](../../commands/release.md), [versioning.md](./versioning.md) |
| update-docs    | `/update-docs`    | Sync documentation with code changes (pre-commit) | [commands/update-docs.md](../../commands/update-docs.md), [agents/docs-updater.md](../../agents/docs-updater.md) |
| clean_gone     | `/clean_gone`     | Clean up local branches deleted on remote        | See [git-workflow.md](./git-workflow.md) |
| code-review    | `/code-review`    | Review a pull request                            | See [agents.md](./agents.md) |
| review-pr      | `/review-pr`      | Comprehensive PR review using specialized agents | See [agents.md](./agents.md) |

### Sentry (Error Monitoring)

| Skill              | Command               | Purpose                                             |
| ------------------ | --------------------- | --------------------------------------------------- |
| getIssues          | `/getIssues`          | Fetch recent Sentry issues, optionally by project   |
| seer               | `/seer`               | Natural language questions about Sentry environment |
| sentry-code-review | `/sentry-code-review` | Analyze Sentry comments on GitHub PRs               |
| sentry-setup-\*    | `/sentry-setup-*`     | Setup tracing, logging, metrics, or AI monitoring   |

## Skills vs Agents: When to Use Which

Some capabilities overlap. Use this to pick the right tool:

| Task                   | Use Skill                                   | Use Agent                                       |
| ---------------------- | ------------------------------------------- | ----------------------------------------------- |
| Plan a complex feature | `/feature-dev` (guided, interactive)        | `planner` agent (autonomous, returns plan)      |
| Review code changes    | `/code-review` or `/review-pr` (PR-focused) | `code-reviewer` agent (file-focused, post-edit) |
| Build UI components    | `/frontend-design` (full design workflow)   | -                                               |
| Fix build errors       | -                                           | `build-error-resolver` agent                    |
| Security analysis      | -                                           | `security-reviewer` agent                       |
| TDD workflow           | -                                           | `tdd-guide` agent                               |
| Dead code cleanup      | -                                           | `refactor-cleaner` agent                        |
| Sustained coding       | `/ralph-loop` (iterative loop)              | -                                               |
| Git commit/PR          | `/commit`, `/commit-push-pr`                | -                                               |

**General rule:** Skills are interactive workflows invoked by the user. Agents are autonomous workers dispatched by Claude. Use skills when the user asks for a workflow; use agents proactively when completing a task.

## Related Documentation

- [agents.md](./agents.md) - Agent selection and orchestration
- [performance.md](./performance.md) - Model selection for agents
- [git-workflow.md](./git-workflow.md) - Commit format and branching
- [versioning.md](./versioning.md) - Release workflow and SemVer
- [commands/release.md](../../commands/release.md) - `/release` skill details
