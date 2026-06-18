import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, describe, expect, it, vi } from "vitest";
import { SocialLoginButtons } from "./social-login-buttons";
import type { EnabledSocialProviders } from "@/lib/auth/social-config";
import { socialProvidersKeys } from "@/lib/api/social-providers.keys";

const socialSignIn = vi.fn().mockResolvedValue({ data: {}, error: null });

vi.mock("@/lib/auth-client", () => ({
  signIn: { social: (...args: Array<unknown>) => socialSignIn(...args) },
}));

// Keep the createServerFn module out of the test import graph — its
// @tanstack/react-start runtime nulls out React under Vitest. The query cache
// is pre-seeded below, so the real server fn is never called anyway.
vi.mock("@/lib/auth/social-providers", () => ({
  default: {},
  fetchEnabledSocialProviders: vi.fn(),
}));

function renderButtons(enabled: EnabledSocialProviders) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  queryClient.setQueryData(socialProvidersKeys.enabled, enabled);
  return render(
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <SocialLoginButtons />
      </MantineProvider>
    </QueryClientProvider>,
  );
}

describe("SocialLoginButtons", () => {
  afterEach(() => {
    socialSignIn.mockClear();
  });

  const googleButton = () =>
    screen.getByRole<HTMLButtonElement>("button", { name: /google/i });
  const facebookButton = () =>
    screen.getByRole<HTMLButtonElement>("button", { name: /facebook/i });

  it("disables both buttons when no providers are configured", () => {
    renderButtons({ google: false, facebook: false });
    expect(googleButton().disabled).toBe(true);
    expect(facebookButton().disabled).toBe(true);
  });

  it("enables only configured providers", () => {
    renderButtons({ google: true, facebook: false });
    expect(googleButton().disabled).toBe(false);
    expect(facebookButton().disabled).toBe(true);
  });

  it("starts the OAuth flow for the clicked provider", async () => {
    const user = userEvent.setup();
    renderButtons({ google: true, facebook: false });

    await user.click(googleButton());

    expect(socialSignIn).toHaveBeenCalledTimes(1);
    expect(socialSignIn).toHaveBeenCalledWith(
      expect.objectContaining({ provider: "google" }),
    );
  });
});
