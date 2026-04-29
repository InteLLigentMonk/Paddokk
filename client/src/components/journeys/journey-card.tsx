import {
  Card,
  Image,
  Stack,
  Group,
  Text,
  ActionIcon,
  Menu,
  Badge,
} from "@mantine/core";
import {
  MoreVertical,
  Edit,
  Trash,
  Heart,
  Bell,
  MessageSquare,
  Pause,
  Play,
  CheckCircle,
  Archive,
  Star,
} from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { JourneyDto, JourneyStatus } from "@/generated/api/schemas";
import {
  openEditJourneyModal,
  openDeleteJourneyConfirm,
} from "@/lib/stores/journeys-page-store";
import {
  updateJourneyFn,
  setDefaultActiveJourneyFn,
} from "@/lib/api/user-journeys.server";
import { useNotifications } from "@/integrations/mantine";

interface JourneyCardProps {
  journey: JourneyDto;
  isDefault?: boolean;
}

const STATUS_ACTIVE: JourneyStatus = 1;
const STATUS_COMPLETED: JourneyStatus = 2;
const STATUS_ON_HOLD: JourneyStatus = 3;
const STATUS_ARCHIVED: JourneyStatus = 4;

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
  const journeyId = Number(journey.id);

  const carName =
    journey.carNickname ||
    [journey.carMakeName, journey.carModelName, journey.carYear]
      .filter(Boolean)
      .join(" ") ||
    "Okänd bil";

  const statusMutation = useMutation({
    mutationFn: (vars: {
      newStatus: JourneyStatus;
      completedAt?: string | null;
    }) =>
      updateJourneyFn({
        data: {
          journeyId,
          status: vars.newStatus,
          completedAt: vars.completedAt,
        },
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-journeys"] });
      notifications.success({ message: "Resans status uppdaterad" });
    },
    onError: () => {
      notifications.error({ message: "Kunde inte uppdatera status" });
    },
  });

  const setDefaultMutation = useMutation({
    mutationFn: () => setDefaultActiveJourneyFn({ data: { journeyId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-journeys"] });
      queryClient.invalidateQueries({ queryKey: ["default-active-journey"] });
      notifications.success({ message: "Aktiv resa uppdaterad" });
    },
    onError: () => {
      notifications.error({ message: "Kunde inte sätta som aktiv resa" });
    },
  });

  const transitionTo = (newStatus: JourneyStatus, completedAt?: string) => {
    statusMutation.mutate({ newStatus, completedAt });
  };

  return (
    <Card
      shadow="sm"
      padding="sm"
      radius="md"
      withBorder
      style={{ cursor: "pointer" }}
      onClick={() =>
        navigate({
          to: "/journeys/$journeyId",
          params: { journeyId: String(journey.id) },
        })
      }
    >
      <Card.Section>
        <Image
          src={
            journey.primaryImageUrl ||
            "https://placehold.co/600x400/e9ecef/495057?text=No+Cover"
          }
          h={250}
          alt={journey.title}
          fit="cover"
        />
      </Card.Section>

      <Stack gap="sm" mt="sm" justify="space-between" style={{ flex: 1 }}>
        <Group justify="space-between" align="flex-start" wrap="nowrap">
          <Stack gap={6} style={{ flex: 1, minWidth: 0 }}>
            <Group gap={6} wrap="wrap">
              <Badge
                variant="light"
                color={STATUS_COLORS[status] ?? "gray"}
                size="sm"
              >
                {STATUS_LABELS[status] ?? "Okänd"}
              </Badge>
              <Badge variant="outline" size="sm">
                {CATEGORY_LABELS[Number(journey.category)] ?? "Övrigt"}
              </Badge>
              {isDefault && (
                <Badge
                  variant="filled"
                  color="violet"
                  size="sm"
                  leftSection={<Star size={10} />}
                >
                  Aktiv resa
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
                Redigera
              </Menu.Item>

              {status !== STATUS_ARCHIVED && <Menu.Divider />}

              {status === STATUS_ACTIVE && (
                <Menu.Item
                  leftSection={<Pause size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    transitionTo(STATUS_ON_HOLD);
                  }}
                >
                  Parkera resa
                </Menu.Item>
              )}
              {status === STATUS_ON_HOLD && (
                <Menu.Item
                  leftSection={<Play size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    transitionTo(STATUS_ACTIVE);
                  }}
                >
                  Publicera resa
                </Menu.Item>
              )}
              {(status === STATUS_ACTIVE || status === STATUS_ON_HOLD) && (
                <Menu.Item
                  leftSection={<CheckCircle size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    transitionTo(STATUS_COMPLETED, new Date().toISOString());
                  }}
                >
                  Slutför resa
                </Menu.Item>
              )}
              {status !== STATUS_ARCHIVED && (
                <Menu.Item
                  leftSection={<Archive size={16} />}
                  onClick={(e) => {
                    e.stopPropagation();
                    transitionTo(STATUS_ARCHIVED);
                  }}
                >
                  Arkivera
                </Menu.Item>
              )}

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
                    Sätt som aktiv resa
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
                Ta bort
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
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
              Start: {formatDate(journey.createdAt)}
            </Text>
            {journey.targetCompletedAt && (
              <Text size="xs" c="dimmed">
                Mål: {formatDate(journey.targetCompletedAt)}
              </Text>
            )}
          </Stack>
        </Group>
      </Stack>
    </Card>
  );
}
