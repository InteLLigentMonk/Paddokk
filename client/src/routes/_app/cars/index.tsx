import { createFileRoute } from "@tanstack/react-router"
import { Container, Stack, Group, Pagination, Alert } from "@mantine/core"
import { useMemo, useState, useEffect } from "react"
import { useStore } from "@tanstack/react-store"
import { AlertCircle } from "lucide-react"
import {
  carsPageStore,
  setSortBy,
  openAddCarModal,
} from "@/lib/stores/cars-page-store"
import { sortCars } from "@/lib/utils/sort-cars"
import { CarsHeader } from "@/components/cars/cars-header"
import { CarsSortControl } from "@/components/cars/cars-sort-control"
import { CarsGrid } from "@/components/cars/cars-grid"
import { EditCarModal } from "@/components/cars/edit-car-modal"
import { DeleteCarConfirm } from "@/components/cars/delete-car-confirm"
import { getUserCarsFn } from "@/lib/api/user-cars.server"
import { useQuery } from "@tanstack/react-query"

export const Route = createFileRoute("/_app/cars/")({
  loader: ({ context: { queryClient } }) =>
    queryClient.ensureQueryData({
      queryKey: ["user-cars"],
      queryFn: () => getUserCarsFn(),
    }),
  component: CarsPage,
})

const PAGE_SIZE = 12

function CarsPage() {
  const [page, setPage] = useState(1)

  const { data, isLoading, error } = useQuery({
    queryKey: ["user-cars"],
    queryFn: () => getUserCarsFn(),
  })

  const cars = data?.cars ?? []
  const sortBy = useStore(carsPageStore, (state) => state.sortBy)

  const sortedCars = useMemo(() => sortCars(cars, sortBy), [cars, sortBy])

  const paginatedCars = useMemo(() => {
    const start = (page - 1) * PAGE_SIZE
    return sortedCars.slice(start, start + PAGE_SIZE)
  }, [sortedCars, page])

  const totalPages = Math.ceil(sortedCars.length / PAGE_SIZE)

  useEffect(() => {
    setPage(1)
  }, [sortBy, cars.length])

  return (
    <Container
      size="lg"
      py="xl"
    >
      <Stack gap="xl">
        <CarsHeader />

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
            Failed to load cars. Please try again.
          </Alert>
        ) : (
          <>
            {cars.length > 0 && (
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

      <EditCarModal />
      <DeleteCarConfirm />
    </Container>
  )
}
