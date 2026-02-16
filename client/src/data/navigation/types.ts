import type { LucideIcon } from 'lucide-react'

export interface NavItem {
  id: string
  label: string
  href?: string // Optional for action items
  icon: LucideIcon
  badge?: number // For notification counts
  action?: () => void // For FAB menu items
  group: 'primary' | 'secondary'
}

export interface NavConfig {
  primary: NavItem[]
  secondary: NavItem[]
  fabActions: NavItem[]
}
