import { Avatar, Group, Paper, Text, ThemeIcon } from "@mantine/core";
import { CarFront, CircleCheck, Flag } from "lucide-react";
import { Link } from "@tanstack/react-router";
import { FEED_ITEM_TYPE } from "./feed-item-type";
import type { ReactNode } from "react";
import type { LucideIcon } from "lucide-react";
import type { FeedItemDto } from "@/generated/api/schemas";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";
import { absoluteTime, relativeTime } from "@/lib/feed/feed-time";

interface FeedLifecycleCardProps {
  item: FeedItemDto;
}

/**
 * Renders the three lifecycle events (UserCarCreated, JourneyStarted, JourneyCompleted) as
 * a single compact row — they are structurally identical (actor + verb + a deep-link to one
 * target), so one cohesive component serves all three rather than three near-duplicate files.
 * Lighter than a full post card so events read as punctuation between posts (ADR-0006).
 */
export function FeedLifecycleCard({ item }: FeedLifecycleCardProps) {
  const presentation = lifecyclePresentation(item);
  if (!presentation) return null;

  const { icon: Icon, color, verb, target } = presentation;

  return (
    <Paper withBorder radius="md" p="sm">
      <Group gap="sm" wrap="nowrap">
        <ThemeIcon variant="light" color={color} radius="xl" size="lg">
          <Icon size={18} />
        </ThemeIcon>
        <Avatar
          src={optimizeImageUrl(item.actorAvatarUrl, 80) ?? null}
          radius="xl"
          size="sm"
          alt={item.actorDisplayName}
        />
        <Text size="sm" style={{ flex: 1, minWidth: 0 }}>
          <Link
            to="/users/$username"
            params={{ username: item.actorUsername }}
            style={{
              color: "inherit",
              fontWeight: 600,
              textDecoration: "none",
            }}
          >
            {item.actorDisplayName}
          </Link>{" "}
          <Text span c="dimmed">
            {verb}
          </Text>{" "}
          {target}
        </Text>
        <Text
          size="xs"
          c="dimmed"
          title={absoluteTime(item.createdAt)}
          style={{ flexShrink: 0 }}
        >
          {relativeTime(item.createdAt)}
        </Text>
      </Group>
    </Paper>
  );
}

interface LifecyclePresentation {
  icon: LucideIcon;
  color: string;
  verb: string;
  target: ReactNode;
}

function lifecyclePresentation(
  item: FeedItemDto,
): LifecyclePresentation | null {
  switch (item.type) {
    case FEED_ITEM_TYPE.UserCarCreated:
      return {
        icon: CarFront,
        color: "blue",
        verb: "added a new car",
        target: <UserCarLink item={item} />,
      };
    case FEED_ITEM_TYPE.JourneyStarted:
      return {
        icon: Flag,
        color: "teal",
        verb: "started a journey",
        target: <JourneyLink item={item} />,
      };
    case FEED_ITEM_TYPE.JourneyCompleted:
      return {
        icon: CircleCheck,
        color: "green",
        verb: "completed a journey",
        target: <JourneyLink item={item} />,
      };
    default:
      return null;
  }
}

function UserCarLink({ item }: { item: FeedItemDto }) {
  if (item.userCarSlug == null || item.userCarLabel == null) return null;
  return (
    <Link
      to="/users/$username/cars/$slug"
      params={{ username: item.actorUsername, slug: item.userCarSlug }}
      style={{
        color: "var(--mantine-color-anchor)",
        fontWeight: 500,
        textDecoration: "none",
      }}
    >
      {item.userCarLabel}
    </Link>
  );
}

function JourneyLink({ item }: { item: FeedItemDto }) {
  if (item.journeySlug == null || item.journeyTitle == null) return null;
  return (
    <Link
      to="/users/$username/journeys/$slug"
      params={{ username: item.actorUsername, slug: item.journeySlug }}
      style={{
        color: "var(--mantine-color-anchor)",
        fontWeight: 500,
        textDecoration: "none",
      }}
    >
      {item.journeyTitle}
    </Link>
  );
}
