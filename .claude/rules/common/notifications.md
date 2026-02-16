---
scope: project-specific
applies-to: [notifications, mantine, error-handling, tanstack-query]
read-when: [showing-notifications, error-handling, user-feedback]
---

# Notifications

Mantine notifications are integrated globally via `src/integrations/mantine/`. The system provides:

## Usage in React Components

```typescript
import { useNotifications } from "@/integrations/mantine";

function MyComponent() {
  const notifications = useNotifications();

  const handleAction = () => {
    notifications.success({ message: "Changes saved!" });
    notifications.error({ message: "Failed to save" });
    notifications.warning({ message: "Check your input" });
    notifications.info({ message: "Data loaded" });
  };
}
```

## Usage in Non-React Contexts

```typescript
import { notify } from '@/integrations/mantine'

async function apiCall() {
  try {
    await fetch(...)
    notify.success({ message: 'Data loaded' })
  } catch (error) {
    notify.error({ message: error.message })
  }
}
```

## Automatic Error Handling

TanStack Query mutations and queries automatically show error notifications via global error handlers in `src/integrations/tanstack-query/error-handler.ts`. Errors from queries/mutations are displayed automatically unless:

- The error is an `AbortError` (intentionally cancelled)
- A custom `onError` handler is provided (overrides global handler)

## Override Automatic Notifications

```typescript
const mutation = useMutation({
  mutationFn: api.updateUser,
  onError: (error) => {
    // Custom handling overrides global error notification
    notifications.warning({ message: "Please check your input" });
  },
});
```

## Notification Options

All notification methods accept:

- `message` (required): The notification message
- `title` (optional): Custom title (defaults: Success, Error, Warning, Info)
- `autoClose` (optional): Auto-close delay in ms or `false` to disable (default: 4000)
- `withCloseButton` (optional): Show/hide close button (default: true)

## Related Documentation

- [CLAUDE.md](../../../CLAUDE.md) - Notifications overview in architecture section
- [SCENARIOS.md](../../SCENARIOS.md) - "Using Notifications" scenario
- [INDEX.md](../../INDEX.md) - Complete documentation map
