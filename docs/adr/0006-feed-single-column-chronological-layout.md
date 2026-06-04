# The Feed is a single chronological column of varying-richness cards, not a grid

The Feed (PRD #161) renders as **one vertical column of full-width cards in strict `CreatedAt DESC` order**, not as a multi-column grid, masonry wall, or bento layout — even though our *discovery* surfaces (browse Journeys / Cars) deliberately do use a `SimpleGrid` 3-up layout. The Feed is different on purpose.

## Why not bento / masonry

The Feed's load-bearing product guarantee is **strict chronological order** — CONTEXT.md ("Feed") and user story #7: *"so that I always understand why an item is at the top."* That guarantee is what makes Following someone legible: top-to-bottom reads newest-to-oldest, unambiguously.

Multi-column layouts break it two ways:

- **Order becomes unreadable.** Masonry packs columns by shortest height, so item N+1 routinely lands physically *above* item N in an adjacent column. The reader can no longer reconstruct "newest first" by scanning.
- **The layout misranks importance.** A height-packing algorithm decides which item gets the big/hero cell. With six item types of wildly different emotional weight, that means a one-line `specChanged` can land in the hero slot while a `journeyCompleted` milestone (the payoff of a two-year build) gets shoved into a small cell lower down. Prominence would be decided by packing, not by meaning.

So the original "in a bento, how do I pick the big card vs the small ones?" question is **dissolved, not answered**: there are no variable cells to size.

## How we get visual engagement without a grid

The motivation for bento was that a column of identical grey rectangles looks dull. We attack that at its real cause — **uniform card shape** — by varying card *richness by item type*, while keeping one item per row:

- **Tier 1, rich** (`journeyPost`): title + text excerpt + real image gallery. A text-only post stays Tier 1, text-forward, with **no fallback or borrowed imagery** — the image slot simply collapses. The text is the richness; a title-plus-paragraph never collides with a Tier 3 one-liner.
- **Tier 2, medium** (`journeyStarted`, `journeyCompleted`, `userCarCreated`, `photosAdded`): one hero visual that **is the subject** of the event (the Journey cover, the UserCar photo, the added photos) — honest, not borrowed. `photosAdded` renders a horizontal thumbnail strip.
- **Tier 3, slim** (`specChanged`): a one-line diff strip.

The distinction that keeps "no borrowed imagery" coherent: showing a Journey cover on a `journeyStarted` card is honest because the cover **is** what started; showing that same cover behind a text-only `journeyPost` would be borrowed chrome standing in for content the post doesn't have.

A reading-width column (~600px), the **author's avatar on every card** (identity, the strongest scannability anchor — not decoration), **relative timestamps**, and **day dividers** (the only non-card element allowed in the stream) complete the legibility of the chronological order.

## The Feed is a component, not a route

`<FeedStream>` owns the shell + cards + infinite scroll + empty state. `/feed` renders it bare (mobile home); `/dashboard` will later wrap it with a desktop side rail (stats / discovery widgets / quick actions). There is **no viewport redirect** between them — the server has no viewport (the same constraint behind our Mantine SSR dark-mode rule), so a "desktop can't use /feed" redirect could only fire post-hydration and would flash. Both routes work at any width; navigation simply points desktop users at `/dashboard` and mobile users at `/feed`. The **auth redirect stays** (anonymous → redirect, decided server-side from the session).

## Consequences

- Cards are **launchpads**, not interaction surfaces: each deep-links to its source and carries no inline Like/Comment. This also respects the model — Likes are not on JourneyPosts (CONTEXT.md), and inline commenting would drag ADR-0002's Comment/Reply thread rules into every card.
- The side rail and its widgets ("Top5", stats, quick actions) are **out of the Feed slice (#185)** — they are deferred follow-up issues with their own queries, and several may overlap the existing `/dashboard`, so that surface is audited before they are built. Discovery content lives in the rail (clearly-labelled chrome), **never injected into the chronological stream** — this preserves both strict-chrono and the PRD's "no fallback content masquerading as the Feed."
- The grid layout is retained for browse/discovery surfaces, where order is ranked-ish and chronology is not promised. Grid vs column is now a deliberate signal of "ranked vs chronological," not an arbitrary style choice.
- If the product later adopts a scored/ranked Feed (explicitly post-MVP per PRD #161), the "why is this at the top" guarantee changes meaning and this layout decision is revisited rather than eroded.
