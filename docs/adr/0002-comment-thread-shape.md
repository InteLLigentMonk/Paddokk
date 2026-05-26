# JourneyPost comments are shallow with an owner-only Reply

A JourneyPost has zero or many top-level Comments, authored by any User. Each Comment may have **at most one Reply**, and that Reply must be authored by the JourneyPost's owner (the Journey owner). There is no further nesting — a Reply cannot itself be replied to.

We picked this over generic multi-level threading because Paddokk is shaped around the **Journey owner as host**: visitors comment on the build, the owner can field each comment once. This avoids the moderation-and-noise failure modes of deep threads, keeps notification fan-out trivially bounded (a Comment notifies the owner; a Reply notifies the original commenter — no other parties), and produces a conversational shape that matches the product metaphor (a builder narrating their car, with readers reacting).

The underlying schema (`PostComment.ParentCommentId` + a `Replies` navigation collection) *technically* permits general threading, which is exactly why this decision needs recording: a reader of the schema will assume Reddit-style threading and try to relax the validators in `CreateCommentHandler` (`Cannot reply to a reply`, `Only the post owner can reply`). Those validators are load-bearing — they encode the product shape, not arbitrary defensive checks.

## Consequences

- Notification types stay at five (see CONTEXT.md). No `Replied`-to-thread-participants fan-out, no thread-watching feature, no dedup logic for nested replies.
- If at some future point the product needs general threading (e.g. for a new comment surface elsewhere), this decision is revisited rather than incrementally eroded.
