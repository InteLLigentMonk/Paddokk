---
name: frontend-quality
#prettier-ignore
description: UI/UX Expert. Use when building, reviewing, or refactoring any frontend UI component or page. Enforces Mantine conventions, responsive design, theme support, accessibility, and visual verification via Playwright screenshots. Never attempt to implement or ship UI changes without invoking this skill first.
model: opus
triggers:
  - TRIGGER AUTOMATICALLY when building or changing any frontend component, page, or UI element
  - TRIGGER AUTOMATICALLY after implementing UI — take Playwright screenshots to verify visually
  - DO NOT ship UI changes without running the visual verification checklist
---

# Frontend Quality

Every UI change must follow these standards and be visually verified. No shipping blind.

## Mantine — No Raw HTML

Use Mantine components for everything. Never reach for raw HTML when Mantine has a component.

| Instead of... | Use... |
|---|---|
| `<button>` | `<Button>` |
| `<input>` | `<TextInput>`, `<NumberInput>`, `<Select>` |
| `<table>` | `<Table>` |
| `<div>` as card | `<Card>`, `<Paper>` |
| `<div>` as layout | `<Stack>`, `<Group>`, `<Flex>`, `<Grid>` |
| `<div>` as container | `<Container>` |
| `<h1>`–`<h6>` | `<Title order={1}>` |
| `<p>`, `<span>` | `<Text>` |
| `<a>` | `<Anchor>` or TanStack `<Link>` |
| `<img>` | `<Image>` |
| `<ul>` / `<ol>` | `<List>` |
| Custom modal | `<Modal>` |
| Custom tooltip | `<Tooltip>` |
| Custom loading | `<Skeleton>`, `<Loader>` |
| CSS spacing hacks | Mantine `spacing` props, `<Space>` |

### Styling Rules
- Use Mantine's `style` prop or `styles` API for component-specific overrides
- Use CSS modules (`.module.css`) for complex custom styling
- Never use inline style objects for layout — use Mantine layout components
- Use Mantine theme tokens (`theme.spacing.md`, `theme.colors.blue[6]`) not hardcoded values
- Never use `!important`

## Responsive Design

Use Mantine's responsive props and breakpoint system:

```tsx
// Responsive props — NOT media queries
<Grid>
  <Grid.Col span={{ base: 12, sm: 6, lg: 4 }}>
    <CarCard />
  </Grid.Col>
</Grid>

<Group gap={{ base: 'xs', sm: 'md' }}>
  <Button size={{ base: 'sm', md: 'md' }}>Save</Button>
</Group>

// ✅ CORRECT — SSR-safe, uses CSS display:none
<Box visibleFrom="sm">Desktop nav</Box>
<Box hiddenFrom="sm">Mobile nav</Box>
<Button hiddenFrom="md">Menu</Button>

// ❌ WRONG — causes hydration mismatch with SSR
const isMobile = useMediaQuery('(max-width: 768px)')
{isMobile ? <MobileNav /> : <DesktopNav />}
```

### Breakpoints
- `xs`: 576px (mobile landscape)
- `sm`: 768px (tablet)
- `md`: 992px (small desktop)
- `lg`: 1200px (desktop)
- `xl`: 1408px (wide desktop)

### Rules
- Always design mobile-first — base styles are mobile, then override up
- Never use JS-based conditional rendering for responsive layout (causes SSR hydration mismatches)
- Always use Mantine's `visibleFrom` / `hiddenFrom` props — they use CSS `display: none` which is SSR-safe
- Touch targets minimum 44x44px on mobile
- No horizontal scroll on any breakpoint
- Test at minimum: 375px (phone), 768px (tablet), 1280px (desktop)

## Dark/Light Theme

Paddokk supports both themes. Every component must work in both.

```tsx
// Use Mantine's color scheme system
import { useMantineColorScheme, useComputedColorScheme } from '@mantine/core'

// For conditional styling
const computedColorScheme = useComputedColorScheme('dark')
const isDark = computedColorScheme === 'dark'

// In CSS modules — use Mantine's light/dark mixins
.card {
  background: light-dark(var(--mantine-color-white), var(--mantine-color-dark-6));
  border: 1px solid light-dark(var(--mantine-color-gray-3), var(--mantine-color-dark-4));
}
```

### Rules
- Never hardcode colors — always use Mantine theme tokens or `light-dark()`
- Text must have sufficient contrast in both themes (WCAG AA: 4.5:1)
- Test every component in both themes before marking done
- Images and icons must be visible in both themes (use `currentColor` for icons)

## Accessibility

### Required on Every Component
- All interactive elements must be keyboard accessible (Tab, Enter, Escape)
- All images must have `alt` text (decorative images use `alt=""`)
- Form inputs must have associated labels (Mantine does this automatically with `label` prop)
- Error messages must be linked to inputs via `aria-describedby` (Mantine's `error` prop handles this)
- Focus must be visible — never remove focus outlines without replacing them

### Landmarks and Structure
- Use semantic HTML via Mantine: `<AppShell>`, `<main>`, `<nav>`
- Headings must follow hierarchy — never skip levels (h1 → h3)
- Use `aria-label` on icon-only buttons: `<ActionIcon aria-label="Delete car">`

### Dynamic Content
- Loading states: use `aria-busy="true"` on containers
- Toasts/notifications: Mantine's notification system handles `aria-live` automatically
- Modals: Mantine handles focus trapping and `aria-modal` automatically

## Visual Verification with Playwright

After building or changing UI, take a screenshot and verify it looks correct. Never assume.

### Screenshot Script
```typescript
// e2e/visual-check.spec.ts
import { test, expect } from '@playwright/test'

test.describe('Visual Verification', () => {
  const viewports = [
    { name: 'mobile', width: 375, height: 812 },
    { name: 'tablet', width: 768, height: 1024 },
    { name: 'desktop', width: 1280, height: 720 },
  ]

  const themes = ['light', 'dark'] as const

  for (const viewport of viewports) {
    for (const theme of themes) {
      test(`${viewport.name} - ${theme} theme`, async ({ page }) => {
        await page.setViewportSize({ width: viewport.width, height: viewport.height })

        // Set color scheme
        await page.emulateMedia({ colorScheme: theme })

        await page.goto('/path-to-component')
        await page.waitForLoadState('networkidle')

        await page.screenshot({
          path: `screenshots/${viewport.name}-${theme}.png`,
          fullPage: true,
        })
      })
    }
  }
})
```

### Verification Workflow
1. Build or change the component
2. Run the visual check: `pnpm playwright test e2e/visual-check.spec.ts`
3. Open the screenshots and verify:
   - Does it look correct at all 3 breakpoints?
   - Does it work in both light and dark theme?
   - Are there any layout breaks, overflow issues, or cut-off text?
   - Do colors have sufficient contrast?
4. If anything is wrong → fix it → re-screenshot → verify again

### Quick Single Screenshot
For a fast check during development:

```typescript
// Take a quick screenshot of current state
await page.screenshot({ path: 'screenshots/check.png' })
```

### Running Visual Checks
```bash
pnpm playwright test e2e/visual-check.spec.ts    # Full matrix (6 screenshots)
pnpm playwright show-report                        # View results in browser
```

## Checklist

Before any UI work is considered done:

- [ ] All UI uses Mantine components (no raw HTML for standard elements)
- [ ] Responsive at mobile (375px), tablet (768px), desktop (1280px)
- [ ] Works in both light and dark theme
- [ ] Keyboard navigable (Tab through all interactive elements)
- [ ] Images have alt text, icon buttons have aria-labels
- [ ] Playwright screenshots taken at all viewports + themes
- [ ] Screenshots reviewed — no visual issues