import { Group, Select, TagsInput } from "@mantine/core";
import { useNavigate } from "@tanstack/react-router";
import type { CarSortKey } from "@/lib/api/cars.server";

const SORT_OPTIONS: { value: CarSortKey; label: string }[] = [
  { value: "Newest", label: "Nyast" },
  { value: "Relevance", label: "Relevans" },
  { value: "MostLiked", label: "Mest gillade" },
  { value: "MostJourneys", label: "Mest resor" },
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

  return (
    <Group gap="sm" wrap="nowrap" align="flex-start">
      <TagsInput
        placeholder="Sök bilar... (tryck Enter eller komma)"
        value={terms}
        onChange={(newTags) => updateSearch(newTags, sort)}
        splitChars={[",", " "]}
        clearable
        flex={1}
        maxTags={10}
      />
      <Select
        data={SORT_OPTIONS}
        value={sort ?? "Newest"}
        onChange={(val) => updateSearch(terms, (val as CarSortKey) ?? undefined)}
        w={160}
        allowDeselect={false}
      />
    </Group>
  );
}
