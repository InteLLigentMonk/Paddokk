import { Avatar, Group, Paper, Text, ThemeIcon } from "@mantine/core";
import { Wrench } from "lucide-react";
import { Link } from "@tanstack/react-router";
import type { FeedItemDto } from "@/generated/api/schemas";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";
import { absoluteTime, relativeTime } from "@/lib/feed/feed-time";

interface FeedSpecChangedCardProps {
  item: FeedItemDto;
}

/**
 * A UserCar whose build specs evolved (#188). The event derives from UpdatedAt with no stored
 * diff, so the hint is intentionally generic ("updated the specs"); the deep-link takes the
 * reader to the car to see what changed. Compact like the lifecycle events (ADR-0006).
 */
export function FeedSpecChangedCard({ item }: FeedSpecChangedCardProps) {
  return (
    <Paper withBorder radius="md" p="sm">
      <Group gap="sm" wrap="nowrap">
        <ThemeIcon variant="light" color="grape" radius="xl" size="lg">
          <Wrench size={18} />
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
            updated the specs on
          </Text>{" "}
          {item.userCarSlug != null && item.userCarLabel != null && (
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
          )}
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
