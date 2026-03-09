import {
  Card,
  Image,
  Stack,
  Group,
  Text,
  ActionIcon,
  Menu,
  Badge,
  Box,
} from "@mantine/core";
import { MoreVertical, Edit, Trash } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import type { UserCarDto } from "@/generated/api/schemas";
import {
  openEditCarModal,
  openDeleteCarConfirm,
} from "@/lib/stores/cars-page-store";

interface CarCardProps {
  car: UserCarDto;
}

export function CarCard({ car }: CarCardProps) {
  const navigate = useNavigate();
  const displayName = car.nickname || `${car.carMakeName} ${car.carModelName}`;

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

      <Stack gap="sm" mt="sm">
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

        {Number(car.journeyCount) > 0 && (
          <Box>
            <Badge variant="light" size="sm">
              {car.journeyCount}{" "}
              {car.journeyCount === 1 ? "Journey" : "Journeys"}
            </Badge>
          </Box>
        )}
      </Stack>
    </Card>
  );
}
