import type { NotificationDto } from "@/generated/api/schemas";

/**
 * Frontend mirror of the backend `NotificationType` enum (Core/Models/Entities/NotificationType.cs).
 * The wire type is a plain integer (System.Text.Json serializes enums by value), so the value order
 * here must match the backend declaration order exactly.
 */
export const NotificationType = {
  JourneyLiked: 0,
  CarLiked: 1,
  CommentOnYourPost: 2,
  ReplyToYourComment: 3,
  NewFollower: 4,
} as const;

/** The action phrase shown after the actor's name, e.g. "Alice liked your journey". */
export function notificationMessage(notification: NotificationDto): string {
  switch (notification.type) {
    case NotificationType.JourneyLiked:
      return "liked your journey";
    case NotificationType.CarLiked:
      return "liked your car";
    case NotificationType.CommentOnYourPost:
      return "commented on your post";
    case NotificationType.ReplyToYourComment:
      return "replied to your comment";
    case NotificationType.NewFollower:
      return "started following you";
    default:
      return "interacted with you";
  }
}
