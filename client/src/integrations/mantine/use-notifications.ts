import { notifications as mantineNotifications } from "@mantine/notifications";
import type { ReactNode } from "react";

export interface NotificationOptions {
  message: ReactNode;
  title?: string;
  autoClose?: number | false;
  withCloseButton?: boolean;
  /** Stable id, so a notification can later be dismissed via {@link NotificationUtilities.dismiss}. */
  id?: string;
}

export interface NotificationUtilities {
  success: (options: NotificationOptions) => void;
  error: (options: NotificationOptions) => void;
  warning: (options: NotificationOptions) => void;
  info: (options: NotificationOptions) => void;
  /** Hides an open notification by id (e.g. clearing a stale failure toast after a retry). */
  dismiss: (id: string) => void;
}

function createNotificationUtilities(): NotificationUtilities {
  return {
    success: ({ message, title = "Success", ...options }) => {
      mantineNotifications.show({
        title,
        message,
        autoClose: 3000,
        color: "green",
        ...options,
      });
    },

    error: ({ message, title = "Error", ...options }) => {
      mantineNotifications.show({
        title,
        message,
        autoClose: 5000,
        color: "red",
        ...options,
      });
    },

    warning: ({ message, title = "Warning", ...options }) => {
      mantineNotifications.show({
        title,
        message,
        autoClose: 4000,
        color: "yellow",
        ...options,
      });
    },

    info: ({ message, title = "Info", ...options }) => {
      mantineNotifications.show({
        title,
        message,
        autoClose: 3000,
        color: "blue",
        ...options,
      });
    },

    dismiss: (id) => {
      mantineNotifications.hide(id);
    },
  };
}

export const notify = createNotificationUtilities();

export function useNotifications(): NotificationUtilities {
  return notify;
}
