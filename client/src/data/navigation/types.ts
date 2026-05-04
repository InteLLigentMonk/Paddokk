import type { LucideIcon } from "lucide-react";

export interface NavItem {
  id: string;
  label: string;
  href?: string;
  icon: LucideIcon;
  badge?: number;
  action?: () => void;
  desktopOnly?: boolean;
  mobileOnly?: boolean;
  group: "me" | "discover" | "tools";
}

export interface NavConfig {
  me: NavItem[];
  discover: NavItem[];
  tools: NavItem[];
  fabActions: NavItem[];
}
