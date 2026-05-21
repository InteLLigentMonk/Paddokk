import type { LucideIcon } from "lucide-react";

export interface NavHrefContext {
  username: string;
}

export type NavHref = string | ((ctx: NavHrefContext) => string);

export interface NavItem {
  id: string;
  label: string;
  href?: NavHref;
  icon: LucideIcon;
  badge?: number;
  action?: () => void;
  desktopOnly?: boolean;
  mobileOnly?: boolean;
  group: "me" | "discover" | "tools";
}

export interface NavConfig {
  me: Array<NavItem>;
  discover: Array<NavItem>;
  tools: Array<NavItem>;
  fabActions: Array<NavItem>;
}
