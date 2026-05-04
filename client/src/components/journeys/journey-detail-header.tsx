import {
  Box,
  Group,
  Stack,
  Text,
  Title,
  Badge,
  Avatar,
  Image,
  Paper,
  Divider,
} from "@mantine/core";
import { MessageSquare, Heart, Bell } from "lucide-react";
import type { JourneyDto } from "@/generated/api/schemas";
import { ExpandableText } from "@/components/common/expandable-text";

const STATUS_LABELS: Record<number, string> = {
  1: "Aktiv",
  2: "Slutförd",
  3: "Parkerad",
  4: "Arkiverad",
};

const STATUS_COLORS: Record<number, string> = {
  1: "green",
  2: "blue",
  3: "yellow",
  4: "gray",
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
  const status = Number(journey.status);
  const category = Number(journey.category);

  const carLabel =
    journey.carNickname ||
    [journey.carMakeName, journey.carModelName, journey.carYear]
      .filter(Boolean)
      .join(" ") ||
    "Okänd bil";

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
          <Image
            src={journey.primaryImageUrl}
            h={{ base: 200, sm: 280 }}
            fit="cover"
            alt={journey.title}
          />
        </Box>
      )}

      <Stack gap="md" p="lg">
        <Group gap="xs" wrap="wrap">
          <Badge variant="light" color={STATUS_COLORS[status] ?? "gray"}>
            {STATUS_LABELS[status] ?? "Okänd"}
          </Badge>
          <Badge variant="outline">
            {CATEGORY_LABELS[category] ?? "Övrigt"}
          </Badge>
        </Group>

        <Title order={2}>{journey.title}</Title>

        {journey.description && (
          <ExpandableText
            text={journey.description}
            maxLines={2}
            c="dimmed"
            isHtml
          />
        )}

        <Divider />

        <Group justify="space-between" wrap="wrap" gap="md">
          <Group gap="sm" wrap="nowrap">
            <Avatar
              src={journey.carPrimaryImageUrl ?? null}
              radius="sm"
              size="md"
              alt={carLabel}
            />
            <Stack gap={2}>
              <Text fw={500} size="sm">
                {carLabel}
              </Text>
              {journey.carGenerationName && (
                <Text size="xs" c="dimmed">
                  {journey.carGenerationName}
                </Text>
              )}
            </Stack>
          </Group>

          <Group gap="lg" wrap="wrap">
            <Group gap={4}>
              <MessageSquare size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.postCount)} inlägg
              </Text>
            </Group>
            <Group gap={4}>
              <Heart size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.likeCount)} gillningar
              </Text>
            </Group>
            <Group gap={4}>
              <Bell size={16} color="var(--mantine-color-dimmed)" />
              <Text size="sm" c="dimmed">
                {Number(journey.subscriberCount)} följare
              </Text>
            </Group>
          </Group>
        </Group>
      </Stack>
    </Paper>
  );
}
