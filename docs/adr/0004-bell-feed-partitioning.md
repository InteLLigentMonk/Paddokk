# Notifications and Feed are partitioned by interaction type, not by entity

The bell (Notifications) and the home stream (Feed) carry different classes of signal and may not overlap.

- **Notifications** carry *direct interactions*: something was done to **you** or to **your content**. Liked your Journey, commented on your Post, replied to your Comment, followed you. Fan-out is always N=1 and the bell is interruption-class.
- **Feed** carries *ambient activity*: things happening in your network that you may want to see but don't need to act on. JourneyPosts from Followed Users / Subscribed UserCars and Journeys, plus lifecycle events (new UserCar, new Journey) from Followed Users.

The deliberate consequence is that **no event produces both a Notification and a Feed item**. A new JourneyPost from someone you follow appears in your Feed only, never in your bell. A Like on your UserCar appears in your bell only, never in your followers' Feeds. This keeps the bell scarce (so it stays attention-worthy) and the Feed wide (so following someone has discovery value).

We picked this partitioning over the alternative of "ambient-also-notifies" (Instagram-style) because in our scope a single popular Journey with hundreds of Subscribers would produce hundreds of bell pings per JourneyPost, training users to ignore the bell within days of beta. The partitioning is also what makes the Notification module's fan-out trivially bounded — every Notification type has N=1 fan-out, removing the need for any async queue infrastructure at MVP scale.

## Consequences

- If a future product surface needs to push ambient content into the bell (e.g. a "watching this Journey closely" affordance), it is an explicit, opt-in opt-in, not a default — and lands as its own decision.
- The Feed and the Notifications module never share fan-out logic. They are read from the same underlying entities (Like, UserFollow, JourneyPost, etc.) but their projections, pagination, and read-tracking are independent.
- The five Notification types are closed for MVP; adding a sixth is a product decision that should reference this ADR.
