# Notifications join the actor on read instead of snapshotting

A `Notification` row stores only `ActorId`, not a denormalized `ActorUsername` / `ActorDisplayName` / `ActorAvatarUrl`. The actor's display fields are joined from `ApplicationUser` at read time.

The driving reason is GDPR erasure correctness. When a User soft-deletes their account, their `ApplicationUser` row is anonymized in place; a join-on-read approach picks up the anonymization for free on every notification they appear in. A snapshot-at-write approach would freeze the original username/avatar into every prior Notification row, requiring a fan-out back-fill on every account deletion — easy to forget, easy to get wrong, and a recurring compliance hazard. Join-on-read also gives the right product behavior on username/avatar changes: notifications always reflect who the actor *is now*, not who they were.

The performance cost is one join against a hot, indexed table on a paginated read — small in practice, and recoverable later if a real load profile ever shows it as a hotspot.

## Consequences

- `NotificationDto` is assembled by joining `Notification` to `ApplicationUser` in the query layer; no actor columns on the `Notification` table.
- Future denormalization (e.g. caching display fields for popularity-ranked surfaces) is opt-in and explicit, not a default.
- Anonymized users appear in old notifications under their anonymized identity — this is the intended behavior, not a bug.
