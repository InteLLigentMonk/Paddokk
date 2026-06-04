/**
 * Time formatting for the strictly chronological feed.
 *
 * Two concerns, both pure: a compact relative label for each card ("2h ago") and a
 * day-bucket label for the dividers that segment the stream ("Today" / "Yesterday" /
 * "12 May"). Relative-primary with an absolute string available for the title attribute —
 * recency should be legible at a glance without making the reader do date math (ADR-0006).
 */

const MS_PER_DAY = 86_400_000;

/** Compact, relative-primary label for a card timestamp. */
export function relativeTime(iso: string, now: Date = new Date()): string {
  const diffMs = now.getTime() - new Date(iso).getTime();
  const seconds = Math.floor(diffMs / 1000);

  if (seconds < 45) return "just now";
  const minutes = Math.floor(seconds / 60);
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h ago`;
  const days = Math.floor(hours / 24);
  if (days < 7) return `${days}d ago`;
  const weeks = Math.floor(days / 7);
  if (weeks < 5) return `${weeks}w ago`;

  return new Date(iso).toLocaleDateString(undefined, {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

/** Full, unambiguous timestamp for a hover/title attribute. */
export function absoluteTime(iso: string): string {
  return new Date(iso).toLocaleString(undefined, {
    weekday: "short",
    day: "numeric",
    month: "long",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/** Stable local-day key used to detect day boundaries between consecutive items. */
export function dayKey(iso: string): string {
  const d = new Date(iso);
  return `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;
}

/** Human label for a day divider: "Today" / "Yesterday" / a written date. */
export function dayDividerLabel(iso: string, now: Date = new Date()): string {
  const d = new Date(iso);
  const startOfToday = new Date(
    now.getFullYear(),
    now.getMonth(),
    now.getDate(),
  );
  const startOfThatDay = new Date(d.getFullYear(), d.getMonth(), d.getDate());
  const diffDays = Math.round(
    (startOfToday.getTime() - startOfThatDay.getTime()) / MS_PER_DAY,
  );

  if (diffDays === 0) return "Today";
  if (diffDays === 1) return "Yesterday";

  return d.toLocaleDateString(undefined, {
    weekday: "long",
    day: "numeric",
    month: "long",
    ...(d.getFullYear() === now.getFullYear() ? {} : { year: "numeric" }),
  });
}
