import { Anchor, Breadcrumbs, Text } from "@mantine/core";
import { Link, useMatches } from "@tanstack/react-router";
import { ChevronRight } from "lucide-react";

interface PageBreadcrumbsProps {
  current?: string;
}

export function PageBreadcrumbs({ current }: PageBreadcrumbsProps) {
  const matches = useMatches();
  const parentCrumbs = matches
    .map((m) => {
      const raw = m.staticData?.breadcrumb;
      if (typeof raw === "string") return { label: raw, pathname: m.pathname };
      if (typeof raw === "function") {
        return { label: raw(m.loaderData), pathname: m.pathname };
      }
      return null;
    })
    .filter((c): c is { label: string; pathname: string } => c !== null);

  const crumbs = current
    ? [...parentCrumbs, { label: current, pathname: "" }]
    : parentCrumbs;

  if (crumbs.length < 1) return null;

  return (
    <Breadcrumbs separator={<ChevronRight size={14} />}>
      {crumbs.map((crumb, i) =>
        i < crumbs.length - 1 ? (
          <Anchor
            key={crumb.pathname}
            component={Link}
            to={crumb.pathname}
            c="dimmed"
            size="sm"
          >
            {crumb.label}
          </Anchor>
        ) : (
          <Text key={crumb.pathname || crumb.label} size="sm">
            {crumb.label}
          </Text>
        ),
      )}
    </Breadcrumbs>
  );
}
