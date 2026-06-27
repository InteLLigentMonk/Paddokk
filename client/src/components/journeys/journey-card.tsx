import {
  ActionIcon,
  AspectRatio,
  Badge,
  Card,
  Group,
  Menu,
  Stack,
  Text,
} from "@mantine/core";
import {
  Bell,
  CheckCircle,
  Edit,
  Eye,
  EyeOff,
  Heart,
  MessageSquare,
  MoreVertical,
  Star,
  Trash,
} from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type {
  JourneyActivityTier,
  JourneyDto,
  JourneyStatus,
} from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import {
  openDeleteJourneyConfirm,
  openEditJourneyModal,
} from "@/lib/stores/journeys-page-store";
import {
  setDefaultActiveJourneyFn,
  updateJourneyFn,
} from "@/lib/api/user-journeys";
import { journeyKeys } from "@/lib/api/journeys.keys";
import { useNotifications } from "@/integrations/mantine";

interface JourneyCardProps {
  journey: JourneyDto;
  isDefault?: boolean;
}

const STATUS_ACTIVE: JourneyStatus = 1;
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

function formatDate(iso?: string | null): string {
  if (!iso) return "";
  const date = new Date(iso);
  return date.toLocaleDateString("sv-SE", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

export function JourneyCard({ journey, isDefault = false }: JourneyCardProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const notifications = useNotifications();

  const status = Number(journey.status);
  const activityTier = Number(journey.activityTier);
  const journeyId = Number(journey.id);
  const isPublic = journey.isPublic;
  const isOwner = !!journey.isOwner;

  const carName =
    journey.carNickname ||
    [journey.carMakeName, journey.carModelName, journey.carYear]
      .filter(Boolean)
      .join(" ") ||
    "Unknown car";

  const statusMutation = useMutation({
    meta: { suppressGlobalError: true },
    mutationFn: (vars: {
      newStatus?: JourneyStatus;
      completedAt?: string | null;
      newIsPublic?: boolean;
    }) =>
      updateJourneyFn({
        data: {
          journeyId,
          status: vars.newStatus,
          completedAt: vars.completedAt,
          isPublic: vars.newIsPublic,
        },
      }),
    onSuccess: (_, vars) => {
      journeyKeys.userJourneyListRoots.forEach((queryKey) =>
        queryClient.invalidateQueries({ queryKey }),
      );
      queryClient.invalidateQueries({
        queryKey: journeyKeys.defaultActiveJourney,
      });
      const defaultChanged =
        isDefault &&
        vars.newStatus !== undefined &&
        Number(vars.newStatus) !== STATUS_ACTIVE;
      notifications.success({
        message: defaultChanged ? "Active journey updated" : "Journey updated",
      });
    },
    onError: () => {
      notifications.error({ message: "Could not update the journey" });
    },
  });

  const setDefaultMutation = useMutation({
    meta: { suppressGlobalError: true },
    mutationFn: () => setDefaultActiveJourneyFn({ data: { journeyId } }),
    onSuccess: () => {
      journeyKeys.userJourneyListRoots.forEach((queryKey) =>
        queryClient.invalidateQueries({ queryKey }),
      );
      queryClient.invalidateQueries({
        queryKey: journeyKeys.defaultActiveJourney,
      });
      notifications.success({ message: "Active journey updated" });
    },
    onError: () => {
      notifications.error({ message: "Could not set as active journey" });
    },
  });

  return (
    <Card
      shadow="sm"
      padding="sm"
      radius="md"
      withBorder
      style={{ cursor: "pointer" }}
      onClick={() =>
        navigate({
          to: "/users/$username/journeys/$slug",
          params: {
            username: journey.ownerUsername,
            slug: journey.slug,
          },
        })
      }
    >
      <Card.Section>
        <AspectRatio ratio={16 / 9}>
          <CdnImage
            src={journey.primaryImageUrl}
            width={600}
            placeholder="journey"
            alt={journey.title}
            fit="cover"
          />
        </AspectRatio>
      </Card.Section>

      <Stack gap="sm" mt="sm" justify="space-between" style={{ flex: 1 }}>
        <Group justify="space-between" align="flex-start" wrap="nowrap">
          <Stack gap={6} style={{ flex: 1, minWidth: 0 }}>
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
              {!isPublic && (
                <Badge variant="light" color="gray" size="sm">
                  Under Wraps
                </Badge>
              )}
              <Badge variant="outline" size="sm">
                {CATEGORY_LABELS[Number(journey.category)] ?? "Other"}
              </Badge>
              {isDefault && (
                <Badge
                  variant="filled"
                  color="violet"
                  size="sm"
                  leftSection={<Star size={10} />}
                >
                  Primary
                </Badge>
              )}
            </Group>

            <Text fw={600} lineClamp={1}>
              {journey.title}
            </Text>
            <Text size="sm" c="dimmed" lineClamp={1}>
              {carName}
            </Text>
          </Stack>

          {isOwner && (
            <Menu shadow="md" width={220} position="bottom-end">
              <Menu.Target>
                <ActionIcon
                  variant="default"
                  bd={0}
                  size={44}
                  aria-label="Resans alternativ"
                  style={{ flexShrink: 0 }}
                  onClick={(e) => e.stopPropagation()}
                >
                  <MoreVertical size={18} />
                </ActionIcon>
              </Menu.Target>

              <Menu.Dropdown>
                <Menu.Item
                  leftSection={<Edit size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    openEditJourneyModal(journeyId);
                  }}
                >
                  Edit
                </Menu.Item>

                <Menu.Divider />

                {status === STATUS_ACTIVE && (
                  <Menu.Item
                    leftSection={<CheckCircle size={16} />}
                    onClick={(e) => {
                      e.stopPropagation();
                      statusMutation.mutate({
                        newStatus: STATUS_COMPLETED,
                        completedAt: new Date().toISOString(),
                      });
                    }}
                  >
                    Complete Journey
                  </Menu.Item>
                )}

                <Menu.Item
                  leftSection={
                    isPublic ? <EyeOff size={16} /> : <Eye size={16} />
                  }
                  onClick={(e) => {
                    e.stopPropagation();
                    statusMutation.mutate({ newIsPublic: !isPublic });
                  }}
                >
                  {isPublic ? "Put Under Wraps" : "Make Public"}
                </Menu.Item>

                {status === STATUS_ACTIVE && !isDefault && (
                  <>
                    <Menu.Divider />
                    <Menu.Item
                      leftSection={<Star size={16} />}
                      onClick={(e) => {
                        e.stopPropagation();
                        setDefaultMutation.mutate();
                      }}
                    >
                      Set as Active Journey
                    </Menu.Item>
                  </>
                )}

                <Menu.Divider />
                <Menu.Item
                  leftSection={<Trash size={16} />}
                  c="red"
                  onClick={(e) => {
                    e.stopPropagation();
                    openDeleteJourneyConfirm(journeyId);
                  }}
                >
                  Delete
                </Menu.Item>
              </Menu.Dropdown>
            </Menu>
          )}
        </Group>

        <Group justify="space-between" wrap="nowrap" gap="sm">
          <Group gap={12} wrap="nowrap">
            <Group gap={4} wrap="nowrap">
              <MessageSquare size={14} color="var(--mantine-color-dimmed)" />
              <Text size="xs" c="dimmed">
                {Number(journey.postCount)}
              </Text>
            </Group>
            <Group gap={4} wrap="nowrap">
              <Heart size={14} color="var(--mantine-color-dimmed)" />
              <Text size="xs" c="dimmed">
                {Number(journey.likeCount)}
              </Text>
            </Group>
            <Group gap={4} wrap="nowrap">
              <Bell size={14} color="var(--mantine-color-dimmed)" />
              <Text size="xs" c="dimmed">
                {Number(journey.subscriberCount)}
              </Text>
            </Group>
          </Group>

          <Stack gap={2} align="flex-end">
            <Text size="xs" c="dimmed">
              Started: {formatDate(journey.createdAt)}
            </Text>
            {journey.targetCompletedAt && (
              <Text size="xs" c="dimmed">
                Finished: {formatDate(journey.targetCompletedAt)}
              </Text>
            )}
          </Stack>
        </Group>
      </Stack>
    </Card>
  );
}
