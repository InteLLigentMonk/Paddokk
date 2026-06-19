import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import { MantineProvider } from "@mantine/core";
import { CatchBoundary } from "@tanstack/react-router";
import { ErrorPage } from "./error-page";

function Boom(): never {
  throw new Error("deliberate test failure");
}

function renderInBoundary() {
  return render(
    <MantineProvider defaultColorScheme="light">
      <CatchBoundary
        getResetKey={() => "test"}
        errorComponent={({ error, reset }) => (
          <ErrorPage error={error} onReload={reset} />
        )}
      >
        <Boom />
      </CatchBoundary>
    </MantineProvider>,
  );
}

describe("ErrorPage in an error boundary", () => {
  it("catches a thrown error from a child and renders the 500 page", () => {
    renderInBoundary();

    expect(screen.getByText(/something went wrong/i)).toBeDefined();
  });

  it("renders a Report a problem link with a support mailto", () => {
    renderInBoundary();

    const link = screen.getByRole("link", { name: /report a problem/i });
    expect(link.getAttribute("href")).toContain("mailto:support@paddokk.com");
  });

  it("renders a Reload button", () => {
    renderInBoundary();

    expect(screen.getByRole("button", { name: /reload/i })).toBeDefined();
  });
});
