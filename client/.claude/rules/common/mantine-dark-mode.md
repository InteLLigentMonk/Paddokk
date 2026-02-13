# Mantine Dark Mode & SSR

## Critical: No Conditional JSX Based on Color Scheme

Never branch JSX based on `colorScheme` or `computedColorScheme`. This causes hydration mismatches because the server (no localStorage) and client (reads localStorage) initialize to different values.

```tsx
// BAD - hydration mismatch
{computedColorScheme === "dark" ? <Moon /> : <Sun />}

// GOOD - render both, CSS controls visibility
<Box darkHidden><Sun /></Box>
<Box lightHidden><Moon /></Box>
```

`lightHidden` / `darkHidden` are available on all Mantine components (anything extending Box).

**Gotcha:** Never combine `lightHidden`/`darkHidden` with a `display` style prop. The inline style overrides the `display: none` from the CSS class. Just omit the `display` prop.

## SSR Architecture

```
mantineHtmlProps (static)         → data-mantine-color-scheme="light" + suppressHydrationWarning
ColorSchemeScript (blocking <script>) → reads localStorage, resolves scheme, sets attribute before paint
MantineProvider (React)           → manages state via useState(() => manager.get(defaultColorScheme))
```

- `mantineHtmlProps` always hardcodes `data-mantine-color-scheme="light"` regardless of config
- `suppressHydrationWarning` only covers the `<html>` element itself, NOT children
- Mantine uses **localStorage** (not cookies) for persistence (`mantine-color-scheme-value` key)
- Server cannot read localStorage → state always equals `defaultColorScheme` on server

## Required Config

`defaultColorScheme` **must match** between `ColorSchemeScript` and `MantineProvider`. Use `"auto"` for both:

```tsx
// __root.tsx (shell)
<ColorSchemeScript defaultColorScheme="auto" />

// provider.tsx
<MantineProvider defaultColorScheme="auto">
```

`"auto"` is the only safe default for SSR. It defers resolution to `useIsomorphicEffect`, so server and client initial renders match. Using `"light"` or `"dark"` causes hydration errors when localStorage has a different stored value.

## Color Scheme Toggle Pattern

Follow Mantine's official pattern -- no JS branching, CSS-only visibility:

```tsx
import { ActionIcon, Box, useComputedColorScheme, useMantineColorScheme } from "@mantine/core"

export function ColorSchemeToggle() {
  const { setColorScheme } = useMantineColorScheme()
  const computedColorScheme = useComputedColorScheme("light", {
    getInitialValueInEffect: true,
  })

  return (
    <ActionIcon
      variant="default"
      size="lg"
      aria-label="Toggle color scheme"
      onClick={() => setColorScheme(computedColorScheme === "light" ? "dark" : "light")}
    >
      <Box component="span" darkHidden><Sun /></Box>
      <Box component="span" lightHidden><Moon /></Box>
    </ActionIcon>
  )
}
```

`computedColorScheme` is only used in `onClick` (client-only), never for rendering decisions.

## Theme-Aware Styling

| Instead of | Use |
|---|---|
| `var(--mantine-color-gray-0/1/2/3)` | `var(--mantine-color-default-border)` for borders |
| `theme.colors.dark[7]` | `var(--mantine-color-text)` |
| `var(--mantine-color-myColor-6)` | `var(--mantine-primary-color-filled)` (respects `primaryShade`) |
| Hardcoded background | `var(--mantine-color-body)` |
| Custom light-only value | `light-dark(lightValue, darkValue)` CSS function |

`light-dark()` is processed by `postcss-preset-mantine`. Works in `style` props on Mantine components. Example:

```tsx
style={{ backgroundColor: "light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))" }}
```

## primaryShade

Use separate light/dark shades for brand colors with poor dark-mode contrast:

```tsx
createTheme({ primaryShade: { light: 6, dark: 4 } })
```
