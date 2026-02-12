import { z } from 'zod'

export const navLinkSchema = z.object({
  label: z.string(),
  href: z.string(),
  type: z.enum(['anchor', 'route']),
})

export type NavLink = z.infer<typeof navLinkSchema>

export const mainNavLinks: readonly NavLink[] = [
  { label: 'Features', href: '#features', type: 'anchor' },
  { label: 'Pricing', href: '#pricing', type: 'anchor' },
  { label: 'Roadmap', href: '#roadmap', type: 'anchor' },
] as const

export const footerNavLinks: readonly NavLink[] = [
  { label: 'Features', href: '#features', type: 'anchor' },
  { label: 'Pricing', href: '#pricing', type: 'anchor' },
  { label: 'Roadmap', href: '#roadmap', type: 'anchor' },
] as const

export const legalLinks: readonly NavLink[] = [
  { label: 'Privacy Policy', href: '/privacy', type: 'route' },
  { label: 'Terms of Service', href: '/terms', type: 'route' },
] as const
