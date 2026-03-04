import { notifications as mantineNotifications } from "@mantine/notifications";

export interface NotificationOptions {
  message: string;
  title?: string;
  autoClose?: number | false;
  withCloseButton?: boolean;
}

export interface NotificationUtilities {
  success: (options: NotificationOptions) => void;
  error: (options: NotificationOptions) => void;
  warning: (options: NotificationOptions) => void;
  info: (options: NotificationOptions) => void;
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
  };
}

export const notify = createNotificationUtilities();

export function useNotifications(): NotificationUtilities {
  return notify;
}
