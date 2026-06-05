import { useNavigate } from "@tanstack/react-router";
import { useCallback } from "react";
import type { NotificationDto } from "@/generated/api/schemas";
import { useMarkNotificationRead } from "@/lib/api/notifications.queries";

/**
 * The single "activate a notification" gesture shared by the bell and the hub: optimistically
 * mark it read (only if unread) and deep-link to its source. The target is resolved server-side
 * (TargetUrl); the navigation surface renders its own "no longer available" state for deleted
 * content, so an empty target simply does not navigate.
 */
export function useNotificationActivate() {
  const navigate = useNavigate();
  const markRead = useMarkNotificationRead();

  return useCallback(
    (notification: NotificationDto) => {
      if (!notification.read) {
        markRead.mutate(notification.id);
      }
      if (notification.targetUrl) {
        navigate({ to: notification.targetUrl });
      }
    },
    [markRead, navigate],
  );
}
