import {
  Avatar,
  Button,
  Card,
  Divider,
  Group,
  Skeleton,
  Stack,
  Text,
} from "@mantine/core";
import { Link } from "@tanstack/react-router";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { Bell } from "lucide-react";
import type { UserCarDto } from "@/generated/api/schemas";
import { userCarsByUsernameQueryOptions } from "@/lib/api/users.queries";
import {
  subscribeToUserCarFn,
  unsubscribeFromUserCarFn,
} from "@/lib/api/user-cars";
import { useNotifications } from "@/integrations/mantine";

interface CarOwnerGarageProps {
  car: UserCarDto;
}

export function CarOwnerGarage({ car }: CarOwnerGarageProps) {
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const [subscribeLoading, setSubscribeLoading] = useState(false);

  const { data: ownerCars, isLoading } = useQuery(
    userCarsByUsernameQueryOptions(car.ownerUsername),
  );

  const otherCars =
    ownerCars?.filter((c) => String(c.id) !== String(car.id)) ?? [];
  const firstName = car.ownerUsername.split("_")[0] ?? car.ownerUsername;

  const handleSubscribe = async () => {
    setSubscribeLoading(true);
    try {
      if (car.isSubscribed) {
        await unsubscribeFromUserCarFn({ data: { carId: Number(car.id) } });
      } else {
        await subscribeToUserCarFn({ data: { carId: Number(car.id) } });
      }
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
    } catch {
      notifications.error({ message: "Failed to update follow" });
    } finally {
      setSubscribeLoading(false);
    }
  };

  return (
    <Card withBorder radius="md" padding="md">
      {/* Owner row */}
      <Group justify="space-between" mb="sm">
        <Link
          to="/users/$username"
          params={{ username: car.ownerUsername }}
          style={{ textDecoration: "none", color: "inherit" }}
        >
          <Group gap="sm">
            <Avatar src={car.ownerAvatarUrl} size={36} radius="xl" />
            <div>
              <Text fz={13} fw={600} lh={1.2}>
                {car.ownerUsername}
              </Text>
              <Text fz={11} c="dimmed">
                Owner
              </Text>
            </div>
          </Group>
        </Link>
        {!car.isOwner && (
          <Button
            variant="subtle"
            color="light-dark(var(--mantine-color-dark-7), var(--mantine-color-gray-3))"
            bd="1px solid var(--mantine-color-default-border)"
            size="sm"
            leftSection={
              <Bell
                size={14}
                fill={car.isSubscribed ? "var(--mantine-color-blue-5)" : "none"}
                stroke={
                  car.isSubscribed
                    ? "var(--mantine-color-blue-5)"
                    : "currentColor"
                }
              />
            }
            onClick={handleSubscribe}
            loading={subscribeLoading}
          >
            {car.isSubscribed ? "Following" : "Follow"}
          </Button>
        )}
      </Group>

      {otherCars.length > 0 && (
        <>
          <Divider mb="sm" />
          <Text
            ff="monospace"
            tt="uppercase"
            fz={10}
            fw={700}
            c="dimmed"
            lts="0.1em"
            mb={8}
          >
            Also in {firstName}'s garage
          </Text>
          {isLoading ? (
            <Stack gap={6}>
              {[1, 2].map((i) => (
                <Skeleton key={i} h={36} radius="sm" />
              ))}
            </Stack>
          ) : (
            <Stack gap={4}>
              {otherCars.slice(0, 5).map((otherCar) => {
                const name =
                  otherCar.nickname ||
                  (otherCar.isCustomBuild
                    ? (otherCar.customBuildName ?? "Custom Build")
                    : [otherCar.carMakeName, otherCar.carModelName]
                        .filter(Boolean)
                        .join(" "));
                return (
                  <Link
                    key={String(otherCar.id)}
                    to="/users/$username/cars/$slug"
                    params={{
                      username: car.ownerUsername,
                      slug: otherCar.slug,
                    }}
                    style={{ textDecoration: "none", color: "inherit" }}
                  >
                    <Group
                      gap="sm"
                      px={8}
                      py={6}
                      style={{
                        borderRadius: "var(--mantine-radius-sm)",
                        cursor: "pointer",
                      }}
                      className="hover-bg"
                    >
                      <Avatar
                        src={otherCar.primaryImageUrl}
                        size={28}
                        radius="sm"
                      />
                      <div>
                        <Text fz={12} fw={500} lh={1.2}>
                          {name}
                        </Text>
                        {otherCar.year && (
                          <Text fz={10} c="dimmed" ff="monospace">
                            {String(otherCar.year)}
                          </Text>
                        )}
                      </div>
                    </Group>
                  </Link>
                );
              })}
            </Stack>
          )}
        </>
      )}
    </Card>
  );
}
