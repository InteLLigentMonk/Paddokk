import {
  Badge,
  Box,
  Button,
  Group,
  Image,
  ScrollArea,
  Skeleton,
  Stack,
  Text,
  Transition,
} from "@mantine/core";
import { Heart, MessageSquare, Plus } from "lucide-react";
import { Link } from "@tanstack/react-router";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { CarSectionHead } from "./car-section-head";
import { carJourneysQueryOptions } from "@/lib/api/users.queries";

const CATEGORY_LABELS: Record<number, string> = {
  1: "Build & Mods",
  2: "Restoration",
  3: "Racing",
  4: "Adventures",
  5: "Ownership",
};

const CATEGORY_COLORS: Record<number, string> = {
  1: "yellow",
  2: "orange",
  3: "red",
  4: "teal",
  5: "blue",
};

const expandTransition = {
  in: { opacity: 1, transform: "translateY(0)" },
  out: { opacity: 0, transform: "translateY(12px)" },
  transitionProperty: "opacity, transform",
};

function daysAgo(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const days = Math.floor(diff / 86_400_000);
  if (days === 0) return "Today";
  if (days === 1) return "Yesterday";
  if (days < 30) return `${days}d ago`;
  const months = Math.floor(days / 30);
  if (months < 12) return `${months}mo ago`;
  return `${Math.floor(months / 12)}y ago`;
}

interface CarJourneyListProps {
  username: string;
  slug: string;
  isOwner: boolean;
  totalCount: number | string;
}

export function CarJourneyList({
  username,
  slug,
  isOwner,
  totalCount,
}: CarJourneyListProps) {
  const [showAll, setShowAll] = useState(false);
  const total = Number(totalCount);

  const {
    data: journeys,
    isLoading,
    isFetching,
  } = useQuery({
    ...carJourneysQueryOptions(username, slug, 1, showAll ? total : 5),
    placeholderData: keepPreviousData,
  });

  const isExpanding = showAll && isFetching && !!journeys;
  const expandingSkeletonCount = Math.min(total - 5, 10);
  const firstFive = (journeys ?? []).slice(0, 5);
  const extraJourneys = (journeys ?? []).slice(5);

  const renderRow = (journey: (typeof firstFive)[number], index: number) => {
    const category = Number(journey.category);
    return (
      <Link
        key={String(journey.id)}
        to="/users/$username/journeys/$slug"
        params={{ username: journey.ownerUsername, slug: journey.slug }}
        style={{ textDecoration: "none", display: "block" }}
      >
        <Group
          gap="md"
          wrap="nowrap"
          p="sm"
          style={{
            borderRadius: "var(--mantine-radius-md)",
            border: "1px solid var(--mantine-color-default-border)",
            transition: "background 0.15s",
          }}
          className="journey-row"
        >
          <Box
            style={{
              width: 100,
              height: 68,
              borderRadius: "var(--mantine-radius-sm)",
              overflow: "hidden",
              flexShrink: 0,
            }}
          >
            <Image
              src={
                journey.primaryImageUrl ??
                "https://placehold.co/200x136/1a1a1a/555?text="
              }
              alt={journey.title}
              w={100}
              h={68}
              fit="cover"
            />
          </Box>

          <Stack gap={4} style={{ flex: 1, minWidth: 0 }}>
            <Group gap="xs" wrap="nowrap">
              <Text
                ff="monospace"
                fz={10}
                fw={700}
                tt="uppercase"
                c="dimmed"
                style={{ flexShrink: 0 }}
              >
                #{String(index + 1).padStart(2, "0")}
              </Text>
              <Badge
                size="xs"
                variant="light"
                color={CATEGORY_COLORS[category] ?? "gray"}
              >
                {CATEGORY_LABELS[category] ?? "Other"}
              </Badge>
              <Text
                fz={10}
                c="dimmed"
                style={{ marginLeft: "auto", flexShrink: 0 }}
              >
                {daysAgo(journey.updatedAt)}
              </Text>
            </Group>

            <Text fw={600} fz={13} lineClamp={1} c="var(--mantine-color-text)">
              {journey.title}
            </Text>

            <Group gap={10}>
              <Group gap={3} wrap="nowrap">
                <Heart size={11} color="var(--mantine-color-dimmed)" />
                <Text fz={10} c="dimmed">
                  {Number(journey.likeCount)}
                </Text>
              </Group>
              <Group gap={3} wrap="nowrap">
                <MessageSquare size={11} color="var(--mantine-color-dimmed)" />
                <Text fz={10} c="dimmed">
                  {Number(journey.postCount)}
                </Text>
              </Group>
            </Group>
          </Stack>
        </Group>
      </Link>
    );
  };

  return (
    <Box>
      <CarSectionHead
        kicker="The story so far"
        title="Journeys"
        count={total > 0 ? `${journeys?.length ?? 0} of ${total}` : undefined}
        rightAction={
          isOwner ? (
            <Link
              to="/users/$username/journeys"
              params={{ username }}
              style={{ textDecoration: "none" }}
            >
              <Button
                size="xs"
                variant="light"
                leftSection={<Plus size={12} />}
              >
                New entry
              </Button>
            </Link>
          ) : undefined
        }
      />

      <ScrollArea.Autosize
        mah={showAll && total > 10 ? 880 : undefined}
        type="hover"
        style={{
          borderBlock: "1px solid var(--mantine-color-default-border)",
        }}
      >
        <Stack gap="xs" p="md">
          {isLoading
            ? Array.from({ length: 3 }).map((_, i) => (
                <Skeleton key={i} h={80} radius="md" />
              ))
            : firstFive.map((journey, index) => renderRow(journey, index))}

          {isExpanding &&
            Array.from({ length: expandingSkeletonCount }).map((_, i) => (
              <Skeleton key={`expand-${i}`} h={80} radius="md" />
            ))}

          <Transition
            mounted={showAll && !isFetching && extraJourneys.length > 0}
            transition={expandTransition}
            duration={600}
            timingFunction="ease"
          >
            {(styles) => (
              <Stack gap="xs" style={styles}>
                {extraJourneys.map((journey, index) =>
                  renderRow(journey, index + 5),
                )}
              </Stack>
            )}
          </Transition>
        </Stack>
      </ScrollArea.Autosize>

      {total > 5 && (
        <Box mt="md" ta="center">
          <Button
            variant="subtle"
            size="sm"
            onClick={() => setShowAll((prev) => !prev)}
          >
            {showAll ? "Show less" : `Show all ${total} journeys`}
          </Button>
        </Box>
      )}

      {!isLoading && total === 0 && (
        <Text fz={13} c="dimmed" py="sm">
          No journeys yet.
        </Text>
      )}
    </Box>
  );
}
