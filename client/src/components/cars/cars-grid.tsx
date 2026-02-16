import { SimpleGrid, Skeleton } from '@mantine/core'
import type { UserCarDto } from '@/generated/api'
import { CarCard } from './car-card'
import { CarsEmptyState } from './cars-empty-state'

interface CarsGridProps {
  cars: UserCarDto[]
  isLoading?: boolean
  onAddCar: () => void
}

export function CarsGrid({ cars, isLoading, onAddCar }: CarsGridProps) {
  if (isLoading) {
    return (
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
        {Array.from({ length: 6 }).map((_, i) => (
          <Skeleton key={i} height={300} radius="md" />
        ))}
      </SimpleGrid>
    )
  }

  if (cars.length === 0) {
    return <CarsEmptyState onAddCar={onAddCar} />
  }

  return (
    <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
      {cars.map((car) => (
        <CarCard key={car.id} car={car} />
      ))}
    </SimpleGrid>
  )
}
