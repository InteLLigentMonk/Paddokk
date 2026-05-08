import { createFileRoute, notFound, useNavigate } from "@tanstack/react-router";
import {
  Container,
  Stack,
  Title,
  Alert,
  SimpleGrid,
  Skeleton,
  Text,
  Card,
  AspectRatio,
  Image,
  Badge,
  Group,
} from "@mantine/core";
import { AlertCircle, EyeOff } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import {
  getUserByUsernameFn,
  getUserCarsByUsernameFn,
} from "@/lib/api/users.server";
import type { UserCarDto } from "@/generated/api/schemas";

export const Route = createFileRoute("/_app/users/$username/cars/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await getUserByUsernameFn({ data: { username: params.username } });
    } catch {
      throw notFound();
    }
    await queryClient.ensureQueryData({
      queryKey: ["user-cars-by-username", params.username],
      queryFn: () =>
        getUserCarsByUsernameFn({ data: { username: params.username } }),
    });
  },
  component: UserCarsPage,
});

function UserCarsPage() {
  const { username } = Route.useParams();
  const { data: cars, isLoading, error } = useQuery({
    queryKey: ["user-cars-by-username", username],
    queryFn: () => getUserCarsByUsernameFn({ data: { username } }),
  });

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <Title order={2}>@{username} — Cars</Title>

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Fel" color="red">
            Kunde inte ladda bilar. Försök igen.
          </Alert>
        ) : isLoading ? (
          <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
            {Array.from({ length: 6 }).map((_, i) => (
              <Skeleton key={i} height={300} radius="md" />
            ))}
          </SimpleGrid>
        ) : !cars || cars.length === 0 ? (
          <Text c="dimmed">No cars to show.</Text>
        ) : (
          <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
            {cars.map((car) => (
              <CarTile key={car.id} car={car} />
            ))}
          </SimpleGrid>
        )}
      </Stack>
    </Container>
  );
}

function CarTile({ car }: { car: UserCarDto }) {
  const navigate = useNavigate();
  const carName =
    car.nickname ||
    [car.carMakeName, car.carModelName, car.year].filter(Boolean).join(" ") ||
    car.customBuildName ||
    "Car";

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
            alt={carName}
            fit="cover"
          />
        </AspectRatio>
      </Card.Section>
      <Stack gap={4} mt="sm">
        <Group justify="space-between">
          <Text fw={600} lineClamp={1}>{carName}</Text>
          {!car.isPublic && (
            <Badge size="sm" color="gray" leftSection={<EyeOff size={10} />}>
              Private
            </Badge>
          )}
        </Group>
        {car.description && (
          <Text size="sm" c="dimmed" lineClamp={2}>
            {car.description}
          </Text>
        )}
      </Stack>
    </Card>
  );
}
