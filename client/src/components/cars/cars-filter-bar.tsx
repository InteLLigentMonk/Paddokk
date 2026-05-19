import { Group, Select, TagsInput } from "@mantine/core";
import { useNavigate } from "@tanstack/react-router";
import type { CarSortKey } from "@/lib/api/cars.server";

const SORT_OPTIONS: { value: CarSortKey; label: string }[] = [
  { value: "Newest", label: "Newest" },
  { value: "Relevance", label: "Relevance" },
  { value: "MostLiked", label: "Most Liked" },
  { value: "MostJourneys", label: "Most Journeys" },
];

interface CarsFilterBarProps {
  terms: string[];
  sort: CarSortKey | undefined;
}

export function CarsFilterBar({ terms, sort }: CarsFilterBarProps) {
  const navigate = useNavigate();

  function updateSearch(newTerms: string[], newSort: CarSortKey | undefined) {
    navigate({
      to: "/cars",
      search: {
        q: newTerms.length > 0 ? newTerms : undefined,
        sort: newSort,
      },
    });
  }

  const effectiveSortKey: CarSortKey = sort ?? (terms.length > 0 ? "Relevance" : "Newest");

  return (
    <Group
      gap="sm"
      wrap="nowrap"
      bg="light-dark(var(--mantine-color-white), var(--mantine-color-dark-7))"
      p="sm"
      align="flex-start"
      style={{ position: "sticky", top: 0, zIndex: 100 }}
    >
      <TagsInput
        placeholder="Search by make, model, generation or year..."
        value={terms}
        onChange={(newTags) => updateSearch(newTags, sort)}
        splitChars={[",", " "]}
        clearable
        flex={1}
        maxTags={10}
      />
      <Select
        data={SORT_OPTIONS}
        value={effectiveSortKey}
        onChange={(val) =>
          updateSearch(terms, (val as CarSortKey) ?? undefined)
        }
        w={160}
        allowDeselect={false}
      />
    </Group>
  );
}
