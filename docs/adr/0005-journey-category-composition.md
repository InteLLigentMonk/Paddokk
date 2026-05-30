# Category-specific Journeys are built by composition, not inheritance

A Journey's Category (`Racing`, `Adventures`, `Restoration`, ...) changes what data the Journey can hold. We model that variation by **composition around a single base `Journey` aggregate**, not by C# subclasses or EF inheritance (TPH/TPT).

A Journey keeps one universal base (title, slug, UserCar, JourneyPosts, Subscriptions, Likes, cover, visibility). Category-specific data hangs off it in three composed layers:

1. An **optional, thin per-category core profile** (1:1) that defines the category's identity — e.g. `RacingProfile { Series, Class, Team }`. A category may have no core at all.
2. **Structured calendar/itinerary entries that own an optional narrative JourneyPost** — e.g. a `RaceRound` carrying planned + result fields, linked 0..1 to the `JourneyPost` that writes it up. A planned-but-undone entry has no post; a plain narrative post has no entry. (See the note below on why the entry owns the post and not the reverse.)
3. **Owner-toggled, category-scoped modules** recorded in a `JourneyModule { Type, Enabled, SortOrder }` enablement table, each backed by its own entity (data-owning) or computed from the entries (derived-view). The set of legal module types is scoped to the Category.

## Why not inheritance

We picked composition over `RacingJourney : Journey` (and over EF TPH/TPT) for three load-bearing reasons:

- **EF's discriminator is effectively immutable.** Inheritance would fight any future "fix a miscategorised Journey" path. We are making `Category` immutable for other reasons, but we don't want the persistence strategy to be the thing that forces it.
- **Polymorphic DTOs break the BFF contract.** A discriminated/`oneOf` Journey payload generates fragile Orval + Zod output, violating the strict generated-schema rules in CLAUDE.md (no handwritten fetchers, generated Zod for every backend DTO). Composition keeps every category resource a **monomorphic, generated-typed** endpoint that the Journey detail page fetches by its known Category.
- **The hot paths stay uniform.** Browse, search, Trending, Featured, and the Feed all read the single base `Journey`. Composition leaves those untouched; TPH bloats the row with per-category nullable columns and TPT adds a join to every one of them.

## Why the calendar entry owns the post (not "a round is a post")

An earlier framing made a round simply a typed `JourneyPost`. That cannot represent a **planned future round** (a season's calendar shows rounds not yet raced) without creating empty placeholder JourneyPosts — which would pollute the timeline and emit meaningless Feed items. So the structured entry is primary and owns an optional narrative post. The Feed still keys off `JourneyPost`: a round becomes Feed-visible only when its writeup is attached.

## Consequences

- Adding a new Category means adding a core profile + calendar-entry type + module catalog entry — **without touching** the base Journey browse/search/Feed queries. Categories scale at the edges.
- `Journey.Category` is immutable after creation (change = delete + recreate). The create command takes Category + initial core profile together; the update path rejects Category changes.
- A new universal `Planning` value joins `JourneyStatus` (`Planning | Active | Completed`). The `Planning -> Active` transition — not `Journey.CreatedAt` — is the "Journey started" lifecycle moment the Feed reacts to.
- The `JourneyModuleCatalog` validity rules (which module types are legal per Category) are product rules, not defensive checks — like the comment-shape validators in ADR-0002, they are load-bearing and should not be relaxed without revisiting this decision.
- This is recorded at **Tier 0**: category data is owner-entered free text, a Journey is a personal logbook, and self-reported results raise no trust problem. A future canonical-`Series` entity (Tier 1) must be additive — a nullable `SeriesId` FK alongside a retained `seriesNameText` fallback — so it does not invalidate this decision.
- If the product ever genuinely needs runtime-polymorphic Journey behaviour that composition can't express, this decision is revisited rather than incrementally eroded by sneaking in an EF discriminator.
