import type { UserCarDto } from '@/generated/api'
import type { SortOption } from '@/lib/stores/cars-page-store'

export function sortCars(cars: UserCarDto[], sortBy: SortOption): UserCarDto[] {
  const sorted = [...cars]

  switch (sortBy) {
    case 'newest':
      return sorted.sort(
        (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )

    case 'oldest':
      return sorted.sort(
        (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
      )

    case 'name-asc':
      return sorted.sort((a, b) => {
        const nameA = a.nickname || `${a.carMakeName} ${a.carModelName}`
        const nameB = b.nickname || `${b.carMakeName} ${b.carModelName}`
        return nameA.localeCompare(nameB)
      })

    case 'name-desc':
      return sorted.sort((a, b) => {
        const nameA = a.nickname || `${a.carMakeName} ${a.carModelName}`
        const nameB = b.nickname || `${b.carMakeName} ${b.carModelName}`
        return nameB.localeCompare(nameA)
      })

    case 'year-new':
      return sorted.sort((a, b) => b.year - a.year)

    case 'year-old':
      return sorted.sort((a, b) => a.year - b.year)

    case 'journeys':
      return sorted.sort((a, b) => (b.journeyCount || 0) - (a.journeyCount || 0))

    default:
      return sorted
  }
}
