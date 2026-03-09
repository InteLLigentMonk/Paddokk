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
import { MoreVertical, Edit, Trash, Heart, Bell } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { UserCarDto } from "@/generated/api/schemas";
import { openDeleteCarConfirm } from "@/lib/stores/cars-page-store";
import {
  likeUserCarFn,
  unlikeUserCarFn,
  subscribeToUserCarFn,
  unsubscribeFromUserCarFn,
} from "@/lib/api/user-cars.server";

interface CarCardProps {
  car: UserCarDto;
}

export function CarCard({ car }: CarCardProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const displayName = car.nickname || `${car.carMakeName} ${car.carModelName}`;

  const likeMutation = useMutation({
    mutationFn: () =>
      car.isLiked
        ? unlikeUserCarFn({ data: { carId: Number(car.id) } })
        : likeUserCarFn({ data: { carId: Number(car.id) } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-cars"] }),
  });

  const subscribeMutation = useMutation({
    mutationFn: () =>
      car.isSubscribed
        ? unsubscribeFromUserCarFn({ data: { carId: Number(car.id) } })
        : subscribeToUserCarFn({ data: { carId: Number(car.id) } }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-cars"] }),
  });

  return (
    <Card
      shadow="sm"
      padding="sm"
      radius="md"
      withBorder
      style={{ cursor: "pointer" }}
      onClick={() =>
        navigate({ to: "/cars/$carId", params: { carId: String(car.id) } })
      }
    >
      <Card.Section>
        <Image
          src={
            car.primaryImageUrl ||
            "https://placehold.co/600x400/e9ecef/495057?text=No+Image"
          }
          h={250}
          alt={displayName}
          fit="cover"
        />
      </Card.Section>

      <Stack gap="sm" mt="sm" justify="space-between" style={{ flex: 1 }}>
        <Group justify="space-between" align="flex-start" wrap="nowrap">
          <Stack gap={4} style={{ flex: 1, minWidth: 0 }}>
            <Text fw={600} lineClamp={1}>
              {car.carMakeName} {car.carModelName}
            </Text>
            <Text size="sm" c="dimmed">
              {car.year}
              {car.carGenerationName && ` • ${car.carGenerationName}`}
            </Text>
            {car.nickname && (
              <Text size="sm" c="dimmed" lineClamp={1}>
                "{car.nickname}"
              </Text>
            )}
          </Stack>

          <Menu shadow="md" width={200} position="bottom-end">
            <Menu.Target>
              <ActionIcon
                variant="subtle"
                size={44}
                aria-label="Car actions"
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
                  navigate({
                    to: "/cars/$carId",
                    params: { carId: String(car.id) },
                    search: { edit: true },
                  });
                }}
              >
                Edit
              </Menu.Item>
              <Menu.Item
                leftSection={<Trash size={16} />}
                c="red"
                onClick={(e) => {
                  e.stopPropagation();
                  openDeleteCarConfirm(Number(car.id));
                }}
              >
                Delete
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>

        <Group gap="sm" wrap="nowrap">
          {Number(car.journeyCount) > 0 && (
            <Badge variant="light" size="sm">
              {car.journeyCount}{" "}
              {car.journeyCount === 1 ? "Journey" : "Journeys"}
            </Badge>
          )}

          <Group gap={12} ml="auto" wrap="nowrap">
            <Group gap={4} ml="auto" wrap="nowrap">
              {car.isOwner ? (
                <Heart size={14} color="var(--mantine-color-dimmed)" />
              ) : (
                <ActionIcon
                  variant={car.isLiked ? "filled" : "subtle"}
                  color={car.isLiked ? "red" : "gray"}
                  size="sm"
                  aria-label={car.isLiked ? "Unlike car" : "Like car"}
                  loading={likeMutation.isPending}
                  onClick={(e) => {
                    e.stopPropagation();
                    likeMutation.mutate();
                  }}
                >
                  <Heart size={14} />
                </ActionIcon>
              )}
              <Text size="xs" c="dimmed">
                {Number(car.likeCount)}
              </Text>
            </Group>

            <Group gap={4} wrap="nowrap">
              {car.isOwner ? (
                <Bell size={14} color="var(--mantine-color-dimmed)" />
              ) : (
                <ActionIcon
                  variant={car.isSubscribed ? "filled" : "subtle"}
                  color={car.isSubscribed ? "blue" : "gray"}
                  size="sm"
                  aria-label={
                    car.isSubscribed
                      ? "Unsubscribe from car"
                      : "Subscribe to car"
                  }
                  loading={subscribeMutation.isPending}
                  onClick={(e) => {
                    e.stopPropagation();
                    subscribeMutation.mutate();
                  }}
                >
                  <Bell size={14} />
                </ActionIcon>
              )}
              <Text size="xs" c="dimmed">
                {Number(car.subscriberCount)}
              </Text>
            </Group>
          </Group>
        </Group>
      </Stack>
    </Card>
  );
}
