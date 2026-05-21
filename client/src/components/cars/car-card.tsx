import {
  ActionIcon,
  AspectRatio,
  Badge,
  Card,
  Group,
  Image,
  Menu,
  Stack,
  Text,
} from "@mantine/core";
import { Bell, Edit, Heart, MoreVertical, Star, Trash } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { UserCarDto } from "@/generated/api/schemas";
import { openDeleteCarConfirm } from "@/lib/stores/cars-page-store";
import {
  likeUserCarFn,
  subscribeToUserCarFn,
  unlikeUserCarFn,
  unsubscribeFromUserCarFn,
  updateUserCarFn,
} from "@/lib/api/user-cars";
import { useNotifications } from "@/integrations/mantine";

interface CarCardProps {
  car: UserCarDto;
}

export function CarCard({ car }: CarCardProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const carAny = car as typeof car & {
    isCustomBuild?: boolean;
    customBuildName?: string | null;
  };
  const displayName =
    car.nickname ||
    (carAny.isCustomBuild
      ? (carAny.customBuildName ?? "Custom Build")
      : `${car.carMakeName} ${car.carModelName}`);

  const invalidateUserCars = () =>
    queryClient.invalidateQueries({
      predicate: (q) => {
        const key = q.queryKey[0];
        return key === "user-cars" || key === "user-cars-by-username";
      },
    });

  const likeMutation = useMutation({
    mutationFn: () =>
      car.isLiked
        ? unlikeUserCarFn({ data: { carId: Number(car.id) } })
        : likeUserCarFn({ data: { carId: Number(car.id) } }),
    onSuccess: invalidateUserCars,
  });

  const subscribeMutation = useMutation({
    mutationFn: () =>
      car.isSubscribed
        ? unsubscribeFromUserCarFn({ data: { carId: Number(car.id) } })
        : subscribeToUserCarFn({ data: { carId: Number(car.id) } }),
    onSuccess: invalidateUserCars,
  });

  const setPrimaryMutation = useMutation({
    mutationFn: () =>
      updateUserCarFn({ data: { carId: Number(car.id), isPrimary: true } }),
    onSuccess: () => {
      invalidateUserCars();
      notifications.success({ message: "Active car updated" });
    },
    onError: () => {
      notifications.error({ message: "Could not set as active car" });
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
          to: "/users/$username/cars/$slug",
          params: { username: car.ownerUsername, slug: car.slug },
        })
      }
    >
      <Card.Section>
        <AspectRatio ratio={16 / 9}>
          <Image
            src={
              car.primaryImageUrl ||
              "https://placehold.co/600x400/e9ecef/495057?text=No+Image"
            }
            alt={displayName}
            fit="cover"
          />
        </AspectRatio>
      </Card.Section>

      <Stack gap="sm" mt="sm" justify="space-between" style={{ flex: 1 }}>
        <Group justify="space-between" align="flex-start" wrap="nowrap">
          <Stack gap={4} style={{ flex: 1, minWidth: 0 }}>
            {car.isPrimary && (
              <Group gap={6}>
                <Badge
                  variant="filled"
                  color="violet"
                  size="sm"
                  leftSection={<Star size={10} />}
                >
                  Active Car
                </Badge>
              </Group>
            )}
            <Text fw={600} lineClamp={1}>
              {carAny.isCustomBuild
                ? (carAny.customBuildName ?? "Custom Build")
                : `${car.carMakeName} ${car.carModelName}`}
            </Text>
            <Text size="sm" c="dimmed">
              {carAny.isCustomBuild
                ? "Custom Build"
                : `${car.year}${car.carGenerationName ? ` â€¢ ${car.carGenerationName}` : ""}`}
            </Text>
            {car.nickname && (
              <Text size="sm" c="dimmed" lineClamp={1}>
                "{car.nickname}"
              </Text>
            )}
          </Stack>

          {car.isOwner && (
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
                      to: "/users/$username/cars/$slug/edit",
                      params: { username: car.ownerUsername, slug: car.slug },
                    });
                  }}
                >
                  Edit
                </Menu.Item>

                {!car.isPrimary && (
                  <>
                    <Menu.Divider />
                    <Menu.Item
                      leftSection={<Star size={16} />}
                      onClick={(e) => {
                        e.stopPropagation();
                        setPrimaryMutation.mutate();
                      }}
                    >
                      Set as Active Car
                    </Menu.Item>
                  </>
                )}

                <Menu.Divider />
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
          )}
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
