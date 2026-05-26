# Paddokk

Automotive community platform. Users own **Cars**, write **Journeys** about them, post updates within those journeys, and react to each other's content through **Likes**, **Comments**, **Subscriptions**, and **Follows**.

## Language

### Actors

**User**:
A registered person. The single human actor in the system; performs every action.
_Avoid_: Account, Member, Customer, Owner (unless qualifying "the owner of this car").

### Content

**UserCar**:
A specific car owned by a User. Distinct from the catalog entities (CarMake, CarModel, CarGeneration), which are reference data.
_Avoid_: "Car" on its own — ambiguous between the owned thing and the catalog entry. Say "UserCar" for the owned thing and "CarMake/CarModel/CarGeneration" for the catalog.

**Journey**:
A long-running story authored by a User about one of their UserCars. Holds many JourneyPosts. Has its own slug, title, cover image, and public/private flag.
_Avoid_: Build, Project, Log, Thread.

**JourneyPost**:
A single update within a Journey. The atomic unit of content in the feed.
_Avoid_: "Post" on its own — ambiguous. Say "JourneyPost".

**Comment**:
A textual top-level remark by any User on a JourneyPost. Each Comment can have **at most one Reply**, and that Reply must be authored by the JourneyPost's owner (the Journey owner). The Reply is the same entity as a Comment (`PostComment` in code, distinguished by `ParentCommentId`), but in product speech it has its own name because the rules around it are different. There is no further nesting — a Reply cannot itself be replied to.
_Avoid_: "PostComment" in product speech (it's the entity name in code; use "Comment" / "Reply" in conversation). "Thread" — there isn't one in the discussion-thread sense.

**Reply**:
The single, owner-authored answer to a Comment. Not a generic threaded reply.
_Avoid_: Using "reply" for the visitor-to-owner direction (that's a Comment); using it for any second-level remark (the owner is the only one who can author a Reply).

### Notifications

**Notification**:
A persistent record that *something happened to you or to your content*. Notifications are direct-interaction-only — ambient events ("someone you follow posted") live in the **Feed**, not the bell. There are exactly five types:

- **JourneyLiked** — another User liked one of your Journeys.
- **CarLiked** — another User liked one of your UserCars.
- **CommentOnYourPost** — another User wrote a Comment on your JourneyPost.
- **ReplyToYourComment** — the JourneyPost owner wrote the Reply to your Comment.
- **NewFollower** — another User started Following you.

_Avoid_: "alert" (loaded with urgency we don't intend), "activity" (overlaps with Feed).

**Feed**:
The chronological stream a User sees on the home screen, composed of **content-advancement events** from the Users / UserCars / Journeys they are connected to via Follow or Subscribe. There are three classes of Feed item:

1. **JourneyPost** — authored by a Followed User, or posted in a Subscribed Journey, or posted in any Journey on a Subscribed UserCar (per Model A).
2. **Lifecycle event** from a Followed User — see entry below.
3. **Content update event** from a Followed User or against a Subscribed UserCar — see entry below.

Ambient signal — not interruption-class. Distinct from **Notifications**.

The Feed is **authenticated-only** — there is no anonymous Feed view. Anonymous visitors to `/feed` are redirected. Logged-in Users with an empty graph (no Follows, no Subscribes) see an honest empty state with CTAs to find people / journeys, not a populated fallback.

_Avoid_: "timeline", "activity feed" (just "Feed").

**Lifecycle event**:
A *state transition* in a User's content. There are three: *UserCar created*, *Journey started*, *Journey completed*. Lifecycle events appear in the Feed of the User's Followers but do **not** produce Notifications (per the bell/feed partitioning).
_Avoid_: "activity event", "system message".

**Content update event**:
A *modification that materially advances* an existing UserCar. There are two: *Photos added* (batched per upload session — one event per command, not per file) and *Spec changed* (one event per successful `UpdateUserCar` save that produced a non-empty diff). Content update events appear in the Feed of the UserCar's Subscribers and the owner's Followers. Like lifecycle events, they do not produce Notifications.
_Avoid_: "Car edit", "Car activity" — the entity-scoped name "Content update event" disambiguates from edits-without-substance (typo fixes etc.) which are deliberately excluded.

**Content-advancement principle**:
The Feed surfaces events that materially *advance* content the User is connected to. Reactions to content (Likes, Comments), edits without substance (note edits, formatting fixes), and relationship events (new Follower, new Subscriber) are deliberately not Feed events. New event proposals should be answered by reference to this principle.

### Discovery surfaces

**Trending Journeys**:
The top 10 Journeys by `JourneySortBy.RecentActivity` (most recently touched). Today this is **not** a velocity-scored / engagement-decay ranking — the word "trending" overpromises what the implementation does. Use it with eyes open; if the product later gets a real trending score, update this entry.
_Avoid_: implying any engagement-velocity computation.

**Featured Journeys**:
The top 10 Journeys by `JourneySortBy.MostLiked` (all-time most-Liked). Today this is **not** editorially curated — the word "featured" overpromises. Use with eyes open.
_Avoid_: implying editorial curation.

### Social relationships

**Subscribe** (verb) / **Subscription** (noun):
The relationship a User has with a piece of *content* — currently a UserCar or a Journey — when they want to be informed of its activity. Reactivatable (soft-deleted, not destroyed). A User cannot Subscribe to their own content. A UserCar-Subscription is a strict superset of its Journey-Subscriptions: subscribing to a UserCar means "I care about this car, including everything that happens in any of its Journeys".
_Avoid_: Watch, Track, Star, Follow (Follow is reserved for the user-to-user case).

**Follow** (verb) / **UserFollow** (noun, the row):
The relationship a User has with another *User*. Reactivatable on the same semantics as Subscribe. A User cannot Follow themselves.
_Avoid_: Friend (Follow is one-directional), Subscribe (Subscribe is reserved for the user-to-content case).

**Like**:
A one-shot reaction by a User to a UserCar or a Journey. A User cannot Like their own content. (No "Like" exists for JourneyPosts or Comments today.)
_Avoid_: Heart, Upvote, React.

## Example dialogue

> **Dev:** "Should liking a post send a notification to the journey owner?"
> **Domain:** "There's no Like on a JourneyPost — Likes are on the UserCar or the Journey itself. The Comment on a JourneyPost is what triggers a notification to the post's author."
>
> **Dev:** "If I Subscribe to a UserCar, do I get notified about Journeys on it?"
> **Domain:** "Subscribe-to-UserCar means you care about that car. The notification fan-out story for new JourneyPosts on a subscribed UserCar is the kind of thing we should pin down explicitly — don't assume."
>
> **Dev:** "Is following someone the same as subscribing to them?"
> **Domain:** "No. You **Follow** a User. You **Subscribe** to a UserCar or a Journey. Two verbs because the UI says two verbs — but under the hood they reuse the same reactivate-on-redo + self-action-blocked rules."
