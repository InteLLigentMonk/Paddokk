import { Anchor, Breadcrumbs, Text } from "@mantine/core";
import { Link, useMatches } from "@tanstack/react-router";

interface PageBreadcrumbsProps {
  current?: string;
}

export function PageBreadcrumbs({ current }: PageBreadcrumbsProps) {
  const matches = useMatches();
  const parentCrumbs = matches
    .filter((m) => typeof m.staticData?.breadcrumb === "string")
    .map((m) => ({
      label: m.staticData.breadcrumb as string,
      pathname: m.pathname,
    }));

  const crumbs = current
    ? [...parentCrumbs, { label: current, pathname: "" }]
    : parentCrumbs;

  if (crumbs.length < 1) return null;

  return (
    <Breadcrumbs>
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
