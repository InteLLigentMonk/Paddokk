import type { FeedItemDto } from "@/generated/api/schemas";

/**
 * Numeric discriminator values for {@link FeedItemDto.type}. The backend `FeedItemType`
 * enum serialises as an integer (JsonNumberHandling.Strict), so Orval generates it as a
 * bare `number`; this const restores meaningful names for narrowing. Values must match the
 * C# enum in `Paddokk.Core/Models/DTOs/Feed/FeedItemDto.cs`.
 */
export const FEED_ITEM_TYPE = {
  JourneyPost: 1,
  UserCarCreated: 2,
  JourneyStarted: 3,
  JourneyCompleted: 4,
  PhotosAdded: 5,
  SpecChanged: 6,
} as const;

/**
 * A stable React key for a feed item. Only JourneyPost ships in this slice; later item
 * types extend this with their own identifying id.
 */
export function feedItemKey(item: FeedItemDto): string {
  if (item.type === FEED_ITEM_TYPE.JourneyPost) {
    return `journey-post-${item.journeyPostId}`;
  }
  // Fallback until the remaining item types land — actor + timestamp is unique enough.
  return `${item.type}-${item.actorUsername}-${item.createdAt}`;
}
