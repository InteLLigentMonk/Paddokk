import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MantineProvider } from "@mantine/core";
import { ApiError } from "./api-error";
import { ApiErrorCode } from "./error-resolver";
import { handleUploadError } from "./upload-error";
import type { ReactNode } from "react";

// Capture what copy/severity each upload failure surfaces without rendering the
// real Mantine notification system (portals crash under jsdom — see memory).
const mocks = vi.hoisted(() => ({
  show: vi.fn(),
  hide: vi.fn(),
}));

vi.mock("@mantine/notifications", () => ({
  notifications: { show: mocks.show, hide: mocks.hide },
}));

function lastNotification() {
  const calls = mocks.show.mock.calls;
  return calls[calls.length - 1][0] as {
    color: string;
    message: ReactNode;
    autoClose?: number | false;
    id?: string;
  };
}

/** Renders the notification's message node so its text/buttons can be asserted. */
function renderMessage(message: ReactNode) {
  return render(<MantineProvider>{message}</MantineProvider>);
}

describe("handleUploadError", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("surfaces image-too-large copy for UPLOAD_TOO_LARGE", () => {
    handleUploadError(
      new ApiError(400, "too big", ApiErrorCode.UploadTooLarge),
    );
    const toast = lastNotification();
    expect(toast.color).toBe("red");
    renderMessage(toast.message);
    expect(
      screen.getByText("Your image is too large. Maximum size is 5 MB."),
    ).toBeDefined();
  });

  it("surfaces supported-formats copy for UPLOAD_UNSUPPORTED_FORMAT", () => {
    handleUploadError(
      new ApiError(400, "bad type", ApiErrorCode.UploadUnsupportedFormat),
    );
    renderMessage(lastNotification().message);
    expect(
      screen.getByText("Only JPEG, PNG, and WebP images are supported."),
    ).toBeDefined();
  });

  it("shows a Retry CTA that re-triggers the upload on a network failure", async () => {
    const user = userEvent.setup();
    const onRetry = vi.fn();

    handleUploadError(new TypeError("Failed to fetch"), onRetry);

    const toast = lastNotification();
    // Persistent so the user has time to act on the retry affordance.
    expect(toast.autoClose).toBe(false);
    renderMessage(toast.message);

    const retry = screen.getByRole("button", { name: "Retry" });
    await user.click(retry);

    expect(onRetry).toHaveBeenCalledTimes(1);
    // The stale failure toast is dismissed once the retry fires.
    expect(mocks.hide).toHaveBeenCalledWith(toast.id);
  });

  it("does not show a retry button when no onRetry is supplied", () => {
    handleUploadError(new TypeError("Failed to fetch"));
    renderMessage(lastNotification().message);
    expect(screen.queryByRole("button", { name: "Retry" })).toBeNull();
  });

  it("falls back to a report link for an unmapped API error", () => {
    handleUploadError(new ApiError(500, "boom", "SOME_UNKNOWN_CODE"), vi.fn());
    const toast = lastNotification();
    expect(mocks.show).toHaveBeenCalledTimes(1);
    renderMessage(toast.message);
    // A structured server error is not a connection failure, so no retry.
    expect(screen.queryByRole("button", { name: "Retry" })).toBeNull();
    expect(
      screen.getByRole("link", { name: "Report a problem" }),
    ).toBeDefined();
  });
});
