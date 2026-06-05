import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NotificationBell } from "./notification-bell";
import type {
  NotificationDto,
  PagedResultOfNotificationDto,
} from "@/generated/api/schemas";

const mocks = vi.hoisted(() => ({
  getUnreadCountFn: vi.fn(),
  getNotificationsFn: vi.fn(),
  markNotificationReadFn: vi.fn(),
  markAllReadFn: vi.fn(),
  navigate: vi.fn(),
}));

// notifications.queries imports these server functions from this module.
vi.mock("@/lib/api/notifications", () => ({
  getUnreadCountFn: mocks.getUnreadCountFn,
  getNotificationsFn: mocks.getNotificationsFn,
  markNotificationReadFn: mocks.markNotificationReadFn,
  markAllReadFn: mocks.markAllReadFn,
}));

// The real @mantine/notifications pulls a second React into the Vitest graph; stub it.
vi.mock("@mantine/notifications", () => ({
  notifications: { show: vi.fn() },
}));

// Render Link as a plain anchor and hand out a controllable navigate(); no RouterProvider needed.
vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    ...rest
  }: {
    children: React.ReactNode;
    to?: string;
  } & Record<string, unknown>) => (
    <a href={typeof to === "string" ? to : "#"} {...rest}>
      {children}
    </a>
  ),
  useNavigate: () => mocks.navigate,
}));

function notification(overrides: Partial<NotificationDto> = {}): NotificationDto {
  return {
    id: 1,
    type: 0, // JourneyLiked
    entityType: "Journey",
    entityId: "42",
    targetUrl: "/users/bob/journeys/engine-swap",
    read: false,
    createdAt: "2026-06-04T10:00:00Z",
    actorId: "actor-1",
    actorUsername: "alice",
    actorDisplayName: "Alice Smith",
    actorAvatarUrl: null,
    ...overrides,
  };
}

function page(items: NotificationDto[]): PagedResultOfNotificationDto {
  return {
    items,
    page: 1,
    pageSize: 20,
    totalCount: items.length,
    hasNextPage: false,
  };
}

function renderBell() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <NotificationBell />
      </MantineProvider>
    </QueryClientProvider>,
  );
}

// TODO(#185): unskip. Mounting NotificationBell pulls a second copy of React into the
// Vitest module graph via Mantine's portal/floating-ui components (Menu/Indicator/ScrollArea),
// tripping "Cannot read properties of null (reading 'useState'/'useRef')". Same root cause as
// the skipped FeedStream test; needs a vitest resolve.dedupe / deps.inline fix to run.
describe.skip("NotificationBell", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.getUnreadCountFn.mockResolvedValue(2);
    mocks.getNotificationsFn.mockResolvedValue(page([notification()]));
    mocks.markNotificationReadFn.mockResolvedValue(undefined);
  });

  it("shows the unread badge from the count endpoint", async () => {
    renderBell();
    await waitFor(() => {
      expect(screen.getByText("2")).toBeDefined();
    });
  });

  it("opening the dropdown does NOT mark anything as read", async () => {
    renderBell();

    fireEvent.click(screen.getByRole("button", { name: "Notifications" }));

    // The recent item loads on open...
    await waitFor(() => {
      expect(screen.getByText("Alice Smith")).toBeDefined();
    });

    // ...but no read mutation is fired just from glancing (anti-dark-pattern, story 12).
    expect(mocks.markNotificationReadFn).not.toHaveBeenCalled();
  });

  it("clicking a notification marks it read and deep-links to the source", async () => {
    renderBell();

    fireEvent.click(screen.getByRole("button", { name: "Notifications" }));

    const row = await screen.findByRole("button", {
      name: /Alice Smith liked your journey/i,
    });
    fireEvent.click(row);

    await waitFor(() => {
      expect(mocks.markNotificationReadFn).toHaveBeenCalledWith({
        data: { id: 1 },
      });
    });
    expect(mocks.navigate).toHaveBeenCalledWith({
      to: "/users/bob/journeys/engine-swap",
    });
  });
});
