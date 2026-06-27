import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MantineProvider } from "@mantine/core";
import {
  QueryClient,
  QueryClientProvider,
  useQuery,
} from "@tanstack/react-query";
import { FollowButton } from "./follow-button";
import type { UserDto } from "@/generated/api/schemas";
import {
  currentUserQueryOptions,
  userByUsernameQueryOptions,
} from "@/lib/api/users.queries";

// Mocks must be hoisted so they exist before the mocked module is imported.
const mocks = vi.hoisted(() => ({
  followUserFn: vi.fn(),
  unfollowUserFn: vi.fn(),
  getUserByUsernameFn: vi.fn(),
  getCurrentUserFn: vi.fn(),
  notificationsShow: vi.fn(),
}));

// Factory must cover every export users.queries imports from this module.
vi.mock("@/lib/api/users", () => ({
  followUserFn: mocks.followUserFn,
  unfollowUserFn: mocks.unfollowUserFn,
  getUserByUsernameFn: mocks.getUserByUsernameFn,
  getCurrentUserFn: mocks.getCurrentUserFn,
  changeUsernameFn: vi.fn(),
  deleteCurrentUserFn: vi.fn(),
  getCarJourneysFn: vi.fn(),
  getUserCarBySlugFn: vi.fn(),
  getUserCarsByUsernameFn: vi.fn(),
  getUserJourneyBySlugFn: vi.fn(),
  getUserJourneysByUsernameFn: vi.fn(),
  updateCurrentUserFn: vi.fn(),
}));

vi.mock("@mantine/notifications", () => ({
  notifications: { show: mocks.notificationsShow },
}));

const PROFILE: UserDto = {
  id: "alice-id",
  email: "alice@example.com",
  firstName: "Alice",
  username: "alice",
  displayName: "Alice",
  createdAt: "2026-01-01T00:00:00Z",
  emailConfirmed: true,
  subscriptionTier: 0,
  carCount: 0,
  journeyCount: 0,
  maxCars: 1,
  followerCount: 5,
  followingCount: 2,
  isFollowedByMe: false,
};

const CURRENT_USER: UserDto = {
  ...PROFILE,
  id: "me-id",
  username: "me",
  email: "me@example.com",
};

function Harness() {
  const { data: user } = useQuery(userByUsernameQueryOptions("alice"));
  if (!user) return null;
  return (
    <>
      <FollowButton
        userId={user.id}
        username={user.username}
        isFollowedByMe={user.isFollowedByMe}
      />
      <span data-testid="follower-count">{user.followerCount}</span>
    </>
  );
}

function renderHarness() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });
  queryClient.setQueryData(
    userByUsernameQueryOptions("alice").queryKey,
    PROFILE,
  );
  queryClient.setQueryData(currentUserQueryOptions().queryKey, CURRENT_USER);

  return render(
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <Harness />
      </MantineProvider>
    </QueryClientProvider>,
  );
}

describe("FollowButton", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Refetch after settle reflects the server truth (still not following).
    mocks.getUserByUsernameFn.mockResolvedValue(PROFILE);
    mocks.getCurrentUserFn.mockResolvedValue(CURRENT_USER);
  });

  it("hides the button on your own profile", () => {
    const queryClient = new QueryClient();
    queryClient.setQueryData(
      userByUsernameQueryOptions("me").queryKey,
      CURRENT_USER,
    );
    queryClient.setQueryData(currentUserQueryOptions().queryKey, CURRENT_USER);

    render(
      <QueryClientProvider client={queryClient}>
        <MantineProvider>
          <FollowButton userId="me-id" username="me" isFollowedByMe={false} />
        </MantineProvider>
      </QueryClientProvider>,
    );

    expect(screen.queryByRole("button")).toBeNull();
  });

  it("optimistically shows Unfollow, then rolls back when the server rejects", async () => {
    const user = userEvent.setup();
    // Keep the follow request pending so the optimistic state is observable.
    let rejectFollow!: (reason: unknown) => void;
    mocks.followUserFn.mockReturnValue(
      new Promise((_resolve, reject) => {
        rejectFollow = reject;
      }),
    );

    renderHarness();

    expect(screen.getByRole("button", { name: "Follow alice" })).toBeDefined();
    expect(screen.getByTestId("follower-count").textContent).toBe("5");

    await user.click(screen.getByRole("button", { name: "Follow alice" }));

    // Optimistic: button flips to Unfollow and follower count increments.
    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: "Unfollow alice" }),
      ).toBeDefined();
    });
    expect(screen.getByTestId("follower-count").textContent).toBe("6");

    // Server rejects -> visible rollback to the prior state.
    rejectFollow(new Error("rejected"));

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: "Follow alice" }),
      ).toBeDefined();
    });
    expect(screen.getByTestId("follower-count").textContent).toBe("5");
    // The error toast is now owned by the global mutation handler (ADR-0007), not the
    // mutation's onError — which here only rolls back — so it is not asserted at this level.
  });
});
