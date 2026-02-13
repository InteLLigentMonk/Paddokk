import { describe, expect, it } from "vitest"
import { render, screen } from "@testing-library/react"
import { MantineProvider } from "@mantine/core"
import { ColorSchemeToggle } from "./color-scheme-toggle"

function renderWithProviders(
  defaultColorScheme: "light" | "dark" | "auto" = "light",
) {
  return render(
    <MantineProvider defaultColorScheme={defaultColorScheme}>
      <ColorSchemeToggle />
    </MantineProvider>,
  )
}

describe("ColorSchemeToggle", () => {
  it("renders an action icon button", () => {
    renderWithProviders()
    const button = screen.getByRole("button")
    expect(button).toBeDefined()
  })

  it("has an accessible aria-label", () => {
    renderWithProviders()
    const button = screen.getByRole("button")
    expect(button.getAttribute("aria-label")).toBe("Toggle color scheme")
  })

  it("renders both sun and moon icons (CSS controls visibility)", () => {
    renderWithProviders()
    const button = screen.getByRole("button")
    const svgs = button.querySelectorAll("svg")
    expect(svgs.length).toBe(2)
  })
})
