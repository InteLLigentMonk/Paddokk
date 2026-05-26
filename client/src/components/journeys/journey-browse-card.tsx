import {
  ActionIcon,
  Anchor,
  AspectRatio,
  Avatar,
  Badge,
  Card,
  Group,
  Stack,
  Text,
} from "@mantine/core";
import { Bell, Heart, MessageSquare } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import type {
  JourneyActivityTier,
  JourneyDto,
  JourneyStatus,
} from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";
import {
  useToggleJourneyLike,
  useToggleJourneySubscription,
} from "@/lib/api/journeys.queries";

interface JourneyBrowseCardProps {
  journey: JourneyDto;
}

const STATUS_COMPLETED: JourneyStatus = 2;

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

export function JourneyBrowseCard({ journey }: JourneyBrowseCardProps) {
  const navigate = useNavigate();
  const journeyId = Number(journey.id);
  const status = Number(journey.status);
  const activityTier = Number(journey.activityTier);

  const likeMutation = useToggleJourneyLike(journeyId);
  const subscribeMutation = useToggleJourneySubscription(journeyId);

  const carName =
    journey.carNickname ||
    [journey.carMakeName, journey.carModelName, journey.carYear]
      .filter(Boolean)
      .join(" ") ||
    "Okänd bil";

  const coverImage =
    journey.primaryImageUrl || journey.carPrimaryImageUrl || undefined;

  function handleCardClick() {
    navigate({
      to: "/users/$username/journeys/$slug",
      params: { username: journey.ownerUsername, slug: journey.slug },
    });
  }

  function handleOwnerClick(e: React.MouseEvent) {
    e.stopPropagation();
    navigate({
      to: "/users/$username",
      params: { username: journey.ownerUsername },
    });
  }

  function handleLikeClick(e: React.MouseEvent) {
    e.stopPropagation();
    likeMutation.mutate(!!journey.isLiked);
  }

  function handleSubscribeClick(e: React.MouseEvent) {
    e.stopPropagation();
    subscribeMutation.mutate(!!journey.isSubscribed);
  }

  function handleCardKeyDown(e: React.KeyboardEvent) {
    if (e.target !== e.currentTarget) return;
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      handleCardClick();
    }
  }

  return (
    <Card
      shadow="sm"
      padding="sm"
      radius="md"
      withBorder
      role="link"
      tabIndex={0}
      aria-label={`Visa journey ${journey.title}`}
      style={{ cursor: "pointer" }}
      onClick={handleCardClick}
      onKeyDown={handleCardKeyDown}
    >
      <Card.Section>
        <AspectRatio ratio={16 / 9}>
          <CdnImage
            src={coverImage}
            width={600}
            placeholder="journey"
            alt={journey.title}
            fit="cover"
          />
        </AspectRatio>
      </Card.Section>

      <Stack gap="xs" mt="sm" style={{ flex: 1 }}>
        <Group gap={6} wrap="wrap">
          <Badge
            variant="light"
            color={ACTIVITY_TIER_COLORS[activityTier] ?? "gray"}
            size="sm"
          >
            {ACTIVITY_TIER_LABELS[activityTier] ?? "Unknown"}
          </Badge>
          {status === STATUS_COMPLETED && (
            <Badge variant="light" color="blue" size="sm">
              Complete
            </Badge>
          )}
          <Badge variant="outline" size="sm">
            {CATEGORY_LABELS[Number(journey.category)] ?? "Other"}
          </Badge>
        </Group>

        <Stack gap={2}>
          <Text fw={600} size="sm" lineClamp={1}>
            {journey.title}
          </Text>
          <Text size="xs" c="dimmed" lineClamp={1}>
            {carName}
          </Text>
        </Stack>

        <Group justify="space-between" align="center" wrap="nowrap" mt="auto">
          <Anchor
            component="button"
            type="button"
            onClick={handleOwnerClick}
            underline="never"
            aria-label={`Visa ${journey.ownerUsername}s profil`}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 6,
              minWidth: 0,
              background: "none",
              border: "none",
              padding: 0,
              cursor: "pointer",
            }}
          >
            <Avatar
              src={optimizeImageUrl(journey.userAvatarUrl, 80)}
              size={20}
              radius="xl"
              name={journey.ownerUsername}
            />
            <Text size="xs" c="dimmed" lineClamp={1}>
              {journey.ownerUsername}
            </Text>
          </Anchor>

          <Group gap={8} wrap="nowrap" style={{ flexShrink: 0 }}>
            <Group gap={3} wrap="nowrap">
              <MessageSquare size={12} color="var(--mantine-color-dimmed)" />
              <Text size="xs" c="dimmed">
                {Number(journey.postCount)}
              </Text>
            </Group>

            <Group gap={3} wrap="nowrap">
              {journey.isOwner ? (
                <Heart size={11} color="var(--mantine-color-dimmed)" />
              ) : (
                <ActionIcon
                  variant={journey.isLiked ? "filled" : "subtle"}
                  color={journey.isLiked ? "red" : "gray"}
                  size="xs"
                  aria-label={journey.isLiked ? "Ta bort gillning" : "Gilla"}
                  loading={likeMutation.isPending}
                  onClick={handleLikeClick}
                >
                  <Heart size={11} />
                </ActionIcon>
              )}
              <Text size="xs" c="dimmed">
                {Number(journey.likeCount)}
              </Text>
            </Group>

            <Group gap={3} wrap="nowrap">
              <ActionIcon
                variant={journey.isSubscribed ? "filled" : "subtle"}
                color={journey.isSubscribed ? "blue" : "gray"}
                size="xs"
                aria-label={
                  journey.isSubscribed ? "Avprenumerera" : "Prenumerera"
                }
                loading={subscribeMutation.isPending}
                onClick={handleSubscribeClick}
              >
                <Bell size={11} />
              </ActionIcon>
              <Text size="xs" c="dimmed">
                {Number(journey.subscriberCount)}
              </Text>
            </Group>
          </Group>
        </Group>
      </Stack>
    </Card>
  );
}
