import { Divider } from "@mantine/core";
import { dayDividerLabel } from "@/lib/feed/feed-time";

interface FeedDayDividerProps {
  /** ISO timestamp of the first item in the day this divider introduces. */
  iso: string;
}

/**
 * The only non-card element allowed in the feed stream. Segments the strictly
 * chronological column by day so the ordering is legible at a glance (ADR-0006).
 */
export function FeedDayDivider({ iso }: FeedDayDividerProps) {
  return (
    <Divider
      my="xs"
      label={dayDividerLabel(iso)}
      labelPosition="center"
      styles={{ label: { fontWeight: 600 } }}
    />
  );
}
