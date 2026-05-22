import type { UserCarDto } from "@/generated/api/schemas";
import type { SortOption } from "@/lib/stores/cars-page-store";

export function sortCars(
  cars: Array<UserCarDto>,
  sortBy: SortOption,
): Array<UserCarDto> {
  const sorted = [...cars];

  switch (sortBy) {
    case "newest":
      return sorted.sort(
        (a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
      );

    case "oldest":
      return sorted.sort(
        (a, b) =>
          new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
      );

    case "name-asc":
      return sorted.sort((a, b) => {
        const nameA = a.nickname || `${a.carMakeName} ${a.carModelName}`;
        const nameB = b.nickname || `${b.carMakeName} ${b.carModelName}`;
        return nameA.localeCompare(nameB);
      });

    case "name-desc":
      return sorted.sort((a, b) => {
        const nameA = a.nickname || `${a.carMakeName} ${a.carModelName}`;
        const nameB = b.nickname || `${b.carMakeName} ${b.carModelName}`;
        return nameB.localeCompare(nameA);
      });

    case "year-new":
      return sorted.sort((a, b) => Number(b.year) - Number(a.year));

    case "year-old":
      return sorted.sort((a, b) => Number(a.year) - Number(b.year));

    case "journeys":
      return sorted.sort(
        (a, b) => Number(b.journeyCount) - Number(a.journeyCount),
      );

    default:
      return sorted;
  }
}
