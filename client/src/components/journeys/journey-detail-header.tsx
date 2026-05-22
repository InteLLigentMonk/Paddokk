import { useState } from "react";
import {
  Anchor,
  Badge,
  Box,
  Button,
  Collapse,
  Divider,
  Group,
  Paper,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { Bell, Heart, MessageSquare } from "lucide-react";
import type { JourneyActivityTier, JourneyDto } from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import { OwnerLink } from "@/components/common/owner-link";

const ACTIVITY_TIER_LABELS: Record<JourneyActivityTier, string> = {
  1: "Full Throttle",
  2: "Cruising",
  3: "Slow Lane",
  4: "Crawling",
  5: "Stalled",
};

const ACTIVITY_TIER_COLORS: Record<JourneyActivityTier, string> = {
  1: "green",
  2: "teal",
  3: "yellow",
  4: "orange",
  5: "red",
};

const CATEGORY_LABELS: Record<number, string> = {
  1: "Build & Mods",
  2: "Restoration",
  3: "Racing",
  4: "Adventures",
  5: "Ownership",
};

interface JourneyDetailHeaderProps {
  journey: JourneyDto;
}

export function JourneyDetailHeader({ journey }: JourneyDetailHeaderProps) {
  const [descriptionExpanded, setDescriptionExpanded] = useState(false);
  const status = Number(journey.status);
  const activityTier = Number(journey.activityTier);
  const category = Number(journey.category);
  const STATUS_COMPLETED = 2;

  const carLabel =
    journey.carNickname ||
    [journey.carMakeName, journey.carModelName, journey.carYear]
      .filter(Boolean)
      .join(" ") ||
    "Unknown Car";

  return (
    <Paper
      withBorder
      radius="md"
      p={0}
      bg="light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))"
      style={{ overflow: "hidden" }}
    >
      {journey.primaryImageUrl && (
        <Box pos="relative">
          <CdnImage
            src={journey.primaryImageUrl}
            width={1600}
            h={{ base: 200, sm: 280 }}
            fit="cover"
            alt={journey.title}
          />
        </Box>
      )}

      <Stack gap="md" p="lg">
        <Group gap="xs" wrap="wrap">
          <Badge
            variant="light"
            color={ACTIVITY_TIER_COLORS[activityTier] ?? "gray"}
          >
            {ACTIVITY_TIER_LABELS[activityTier] ?? "Unknown"}
          </Badge>
          {status === STATUS_COMPLETED && (
            <Badge variant="light" color="blue">
              Complete
            </Badge>
          )}
          {!journey.isPublic && (
            <Badge variant="light" color="gray">
              Under Wraps
            </Badge>
          )}
          <Badge variant="outline">
            {CATEGORY_LABELS[category] ?? "Other"}
          </Badge>
        </Group>
        <Group justify="space-between">
          <Title order={2}>{journey.title}</Title>
          <Button
            visibleFrom="sm"
            variant="subtle"
            onClick={() => setDescriptionExpanded((v) => !v)}
          >
            {descriptionExpanded ? "Show less" : "Read more"}
          </Button>
        </Group>
        {journey.description && (
          <Stack gap={4}>
            <Collapse expanded={descriptionExpanded}>
              <Box
                c="dimmed"
                fz="sm"
                dangerouslySetInnerHTML={{ __html: journey.description }}
                style={{ lineHeight: 1.6 }}
              />
            </Collapse>
            <Anchor
              hiddenFrom="sm"
              size="xs"
              onClick={() => setDescriptionExpanded((v) => !v)}
            >
              {descriptionExpanded ? "Show less" : "Read more"}
            </Anchor>
          </Stack>
        )}

        <Divider />

        <Group justify="space-between" wrap="wrap" gap="md">
          <OwnerLink
            target={{
              kind: "car",
              username: journey.ownerUsername,
              slug: journey.carSlug,
            }}
            avatarUrl={journey.carPrimaryImageUrl}
            primaryText={carLabel}
            secondaryText={journey.carGenerationName ?? undefined}
            avatarSize="md"
            avatarRadius="sm"
            primaryTextWeight={500}
          />

          <Group gap="lg" wrap="wrap">
            <Group gap={4}>
              <MessageSquare size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.postCount)} posts
              </Text>
            </Group>
            <Group gap={4}>
              <Heart size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.likeCount)} likes
              </Text>
            </Group>
            <Group gap={4}>
              <Bell size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.subscriberCount)} subscribers
              </Text>
            </Group>
          </Group>
        </Group>
      </Stack>
    </Paper>
  );
}
