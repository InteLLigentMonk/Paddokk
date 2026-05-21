import { createFileRoute, notFound } from "@tanstack/react-router";
import {
  Alert,
  Container,
  Group,
  Pagination,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useStore } from "@tanstack/react-store";
import { AlertCircle } from "lucide-react";
import {
  userByUsernameQueryOptions,
  userCarsByUsernameQueryOptions,
} from "@/lib/api/users.queries";
import { useCurrentUser } from "@/hooks/use-current-user";
import {
  carsPageStore,
  openAddCarModal,
  setSortBy,
} from "@/lib/stores/cars-page-store";
import { sortCars } from "@/lib/utils/sort-cars";
import { CarsHeader } from "@/components/cars/cars-header";
import { CarsSortControl } from "@/components/cars/cars-sort-control";
import { CarsGrid } from "@/components/cars/cars-grid";
import { AddCarModal } from "@/components/cars/add-car-modal";
import { EditCarModal } from "@/components/cars/edit-car-modal";
import { DeleteCarConfirm } from "@/components/cars/delete-car-confirm";

export const Route = createFileRoute("/_app/users/$username/cars/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await queryClient.ensureQueryData(
        userByUsernameQueryOptions(params.username),
      );
    } catch {
      throw notFound();
    }
    await queryClient.ensureQueryData(
      userCarsByUsernameQueryOptions(params.username),
    );
  },
  component: UserCarsPage,
});

const PAGE_SIZE = 12;

function UserCarsPage() {
  const { username } = Route.useParams();
  const { data: currentUser } = useCurrentUser();
  const isOwner = currentUser?.username === username;

  const [page, setPage] = useState(1);

  const {
    data: cars,
    isLoading,
    error,
  } = useQuery(userCarsByUsernameQueryOptions(username));

  const carList = cars ?? [];
  const sortBy = useStore(carsPageStore, (state) => state.sortBy);

  const sortedCars = useMemo(() => sortCars(carList, sortBy), [carList, sortBy]);

  const paginatedCars = useMemo(() => {
    const start = (page - 1) * PAGE_SIZE;
    return sortedCars.slice(start, start + PAGE_SIZE);
  }, [sortedCars, page]);

  const totalPages = Math.ceil(sortedCars.length / PAGE_SIZE);

  useEffect(() => {
    setPage(1);
  }, [sortBy, carList.length]);

  return (
    <Container size="lg" py="xl">
      <Stack gap="sm">
        {isOwner ? (
          <CarsHeader />
        ) : (
          <Stack gap={4}>
            <Title order={2}>@{username} — Cars</Title>
            <Text c="dimmed" size="sm">
              Cars shared by @{username}
            </Text>
          </Stack>
        )}

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
            Failed to load cars. Please try again.
          </Alert>
        ) : (
          <>
            {carList.length > 0 && (
              <Group justify="space-between" align="flex-end" wrap="wrap">
                <CarsSortControl value={sortBy} onChange={setSortBy} />
              </Group>
            )}

            <CarsGrid
              cars={paginatedCars}
              isLoading={isLoading}
              onAddCar={openAddCarModal}
            />

            {totalPages > 1 && (
              <Group justify="center" mt="xl">
                <Pagination
                  total={totalPages}
                  value={page}
                  onChange={setPage}
                />
              </Group>
            )}
          </>
        )}
      </Stack>

      {isOwner && (
        <>
          <AddCarModal />
          <EditCarModal />
          <DeleteCarConfirm />
        </>
      )}
    </Container>
  );
}
