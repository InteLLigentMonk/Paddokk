import { createServerFn } from "@tanstack/react-start";
import {
  NotificationsGetNotificationsQueryParams,
  NotificationsMarkReadParams,
} from "@/generated/api-zod/notifications/notifications.zod";
import {
  notificationsGetNotifications,
  notificationsGetUnreadCount,
  notificationsMarkAllRead,
  notificationsMarkRead,
} from "@/generated/api/notifications/notifications";

/**
 * The authenticated user's notifications, newest first. The recipient is resolved server-side
 * from the session, so a caller can only ever read their own.
 */
export const getNotificationsFn = createServerFn({ method: "GET" })
  .inputValidator(NotificationsGetNotificationsQueryParams)
  .handler(async ({ data }) => await notificationsGetNotifications(data));

/** Unread count for the bell badge. */
export const getUnreadCountFn = createServerFn({ method: "GET" }).handler(
  async () => await notificationsGetUnreadCount(),
);

/** Mark one notification read (idempotent). Scoped server-side to the actor. */
export const markNotificationReadFn = createServerFn({ method: "POST" })
  .inputValidator(NotificationsMarkReadParams)
  .handler(async ({ data }) => {
    await notificationsMarkRead(data.id);
  });

/** Mark every unread notification read for the actor. */
export const markAllReadFn = createServerFn({ method: "POST" }).handler(
  async () => {
    await notificationsMarkAllRead();
  },
);
