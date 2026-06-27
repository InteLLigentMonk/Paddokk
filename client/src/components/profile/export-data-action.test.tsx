import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ExportDataAction } from "./export-data-action";
import { ApiError } from "@/lib/api/api-error";

// Mocks must be hoisted so they exist before the mocked module is imported.
const mocks = vi.hoisted(() => ({
  requestDataExportFn: vi.fn(),
}));

vi.mock("@/lib/api/data-export", () => ({
  requestDataExportFn: mocks.requestDataExportFn,
}));

function renderAction() {
  const queryClient = new QueryClient({
    defaultOptions: { mutations: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <ExportDataAction />
      </MantineProvider>
    </QueryClientProvider>,
  );
}

describe("ExportDataAction", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows the export button in its idle state", () => {
    renderAction();
    expect(
      screen.getByRole("button", { name: /export my data/i }),
    ).toBeDefined();
  });

  it("confirms success and disables the button without reloading", async () => {
    const user = userEvent.setup();
    mocks.requestDataExportFn.mockResolvedValue({ id: "x", status: 0 });
    renderAction();

    await user.click(screen.getByRole("button", { name: /export my data/i }));

    await waitFor(() => {
      expect(screen.getByText(/you'll receive an email/i)).toBeDefined();
    });
    expect(
      screen.getByRole("button", { name: /export requested/i }),
    ).toHaveProperty("disabled", true);
  });

  it("shows the cooldown message on an EXPORT_COOLDOWN conflict and keeps the button enabled", async () => {
    const user = userEvent.setup();
    mocks.requestDataExportFn.mockRejectedValue(
      new ApiError(
        409,
        "A data export was requested recently.",
        "EXPORT_COOLDOWN",
      ),
    );
    renderAction();

    await user.click(screen.getByRole("button", { name: /export my data/i }));

    await waitFor(() => {
      expect(screen.getByText(/requested an export recently/i)).toBeDefined();
    });
    expect(
      screen.getByRole("button", { name: /export my data/i }),
    ).toHaveProperty("disabled", false);
  });

  it("shows a rate-limit message on a 429 response", async () => {
    const user = userEvent.setup();
    mocks.requestDataExportFn.mockRejectedValue(
      new ApiError(429, "Too many requests", "RATE_LIMITED"),
    );
    renderAction();

    await user.click(screen.getByRole("button", { name: /export my data/i }));

    await waitFor(() => {
      expect(screen.getByText(/too fast/i)).toBeDefined();
    });
  });

  it("shows a generic error with a retry on other failures", async () => {
    const user = userEvent.setup();
    mocks.requestDataExportFn.mockRejectedValue(new Error("boom"));
    renderAction();

    await user.click(screen.getByRole("button", { name: /export my data/i }));

    await waitFor(() => {
      expect(screen.getByText(/could not request your export/i)).toBeDefined();
    });
    // Button remains available so the user can retry.
    expect(
      screen.getByRole("button", { name: /export my data/i }),
    ).toHaveProperty("disabled", false);
  });
});
