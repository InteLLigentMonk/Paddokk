import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { FollowList } from "./follow-list";
import type { UserDto } from "@/generated/api/schemas";

const mocks = vi.hoisted(() => ({
  getFollowersFn: vi.fn(),
  getFollowingFn: vi.fn(),
  getCurrentUserFn: vi.fn(),
  followUserFn: vi.fn(),
  unfollowUserFn: vi.fn(),
}));

// Factory must cover every export users.queries imports from this module.
vi.mock("@/lib/api/users", () => ({
  getFollowersFn: mocks.getFollowersFn,
  getFollowingFn: mocks.getFollowingFn,
  getCurrentUserFn: mocks.getCurrentUserFn,
  followUserFn: mocks.followUserFn,
  unfollowUserFn: mocks.unfollowUserFn,
  getUserByUsernameFn: vi.fn(),
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
  notifications: { show: vi.fn() },
}));

// Render the router Link as a plain anchor so no RouterProvider is needed.
vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    params,
    ...rest
  }: {
    children: React.ReactNode;
    to?: string;
    params?: Record<string, string>;
  } & Record<string, unknown>) => (
    <a href={typeof to === "string" ? to : "#"} {...rest}>
      {children}
    </a>
  ),
}));

const base: UserDto = {
  id: "a",
  email: "a@example.com",
  firstName: "Alice",
  username: "alice",
  displayName: "Alice Smith",
  createdAt: "2026-01-01T00:00:00Z",
  emailConfirmed: true,
  subscriptionTier: 0,
  carCount: 0,
  journeyCount: 0,
  maxCars: 1,
  followerCount: 0,
  followingCount: 0,
  isFollowedByMe: false,
};

function renderList(type: "followers" | "following") {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <FollowList userId="target" type={type} />
      </MantineProvider>
    </QueryClientProvider>,
  );
}

describe("FollowList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.getCurrentUserFn.mockResolvedValue({
      ...base,
      id: "me",
      username: "me",
    });
  });

  it("renders each follower as a row linking to their profile", async () => {
    mocks.getFollowersFn.mockResolvedValue({
      items: [base, { ...base, id: "b", username: "bob", displayName: "Bob" }],
      page: 1,
      pageSize: 20,
      totalCount: 2,
      hasNextPage: false,
    });

    renderList("followers");

    await waitFor(() => {
      expect(screen.getByText("Alice Smith")).toBeDefined();
    });
    expect(screen.getByText("@alice")).toBeDefined();
    expect(screen.getByText("@bob")).toBeDefined();

    const links = screen.getAllByRole("link");
    expect(links[0].getAttribute("href")).toBe("/users/$username");
  });

  it("shows an empty state when there are no followers", async () => {
    mocks.getFollowersFn.mockResolvedValue({
      items: [],
      page: 1,
      pageSize: 20,
      totalCount: 0,
      hasNextPage: false,
    });

    renderList("followers");

    await waitFor(() => {
      expect(screen.getByText("No followers yet.")).toBeDefined();
    });
  });

  it("uses the following query and empty copy for the following tab", async () => {
    mocks.getFollowingFn.mockResolvedValue({
      items: [],
      page: 1,
      pageSize: 20,
      totalCount: 0,
      hasNextPage: false,
    });

    renderList("following");

    await waitFor(() => {
      expect(screen.getByText("Not following anyone yet.")).toBeDefined();
    });
    expect(mocks.getFollowingFn).toHaveBeenCalled();
    expect(mocks.getFollowersFn).not.toHaveBeenCalled();
  });
});
