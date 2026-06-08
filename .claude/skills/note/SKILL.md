---
name: note
description: Capture a quick idea, bug, fix, chore, or feature as a lightweight GitHub "issue-note" (concise title + 1-2 sentences) to pick up later. Use when the user wants to jot down an idea, remember something for later, save a note, or says "note this", "capture this", "make a note", or invokes /note. This is NOT a full issue or PRD -- for those, use the to-prd or to-issues skills.
---

# Note

Turn a passing idea (or something noticed mid-session) into a tiny GitHub issue so it isn't lost. Title + 1-2 sentences. Nothing more.

If the user wants a fully-specified issue or PRD, stop and point them at the `to-issues` / `to-prd` skills instead.

## Input

Two sources, both valid:
- **Typed**: the user supplies a sentence or two with the invocation. Use that.
- **From session**: the user points at something just discussed ("note that", "capture this bug"). Pull the idea from recent context.

If the input is too vague to name (e.g. just "/note"), ask one short clarifying question.

## Workflow

1. **Distill** the idea into:
   - **Title**: one concise line, imperative where natural (e.g. "Debounce the marketplace search input"). No type prefix, no period.
   - **Body**: 1-2 sentences capturing the gist and any single key detail. Do not invent scope, acceptance criteria, or implementation steps -- this is a memory aid, not a spec.
2. **Infer the type label** from the nature of the idea (map `fix` -> `bug`):

   | Idea is about...                         | Label      |
   | ---------------------------------------- | ---------- |
   | new capability / enhancement             | `feat`     |
   | something broken / a fix                 | `bug`      |
   | docs / comments / README                 | `docs`     |
   | internal restructuring, no behavior change | `refactor` |
   | deps, config, build, maintenance         | `chore`    |

   If genuinely ambiguous, omit the type label rather than guess.
3. **Ensure the `note` label exists** (only the first time you hit a missing label):
   ```
   gh label list --json name --jq '.[].name' | Select-String -SimpleMatch note
   ```
   If absent: `gh label create note --color fbca04 --description "Quick-capture idea to pick up later"`
4. **Show the draft** to the user -- title, body, and labels -- and ask for confirmation before creating. Honor edits.
5. **Create** on confirmation:
   ```
   gh issue create --title "<title>" --body "<body>" --assignee @me --label note --label <type>
   ```
   Use a here-string for the body if it spans lines (see PowerShell here-string rules).
6. **Report** the issue URL that `gh` prints.

## Notes

- Always `--assignee @me` and always `--label note`. The type label is added on top when known.
- `gh` infers the repo from the clone's git remote -- no `--repo` needed.
- Keep it fast. The whole point is low-friction capture; don't pad the body or over-clarify.
