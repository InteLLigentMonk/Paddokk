import { Select } from "@mantine/core";
import type { SortOption } from "@/lib/stores/cars-page-store";

interface CarsSortControlProps {
  value: SortOption;
  onChange: (value: SortOption) => void;
}

export function CarsSortControl({ value, onChange }: CarsSortControlProps) {
  return (
    <Select
      label="Sort by"
      value={value}
      onChange={(val) => onChange(val as SortOption)}
      data={[
        { value: "newest", label: "Newest First" },
        { value: "oldest", label: "Oldest First" },
        { value: "name-asc", label: "Name (A-Z)" },
        { value: "name-desc", label: "Name (Z-A)" },
        { value: "year-new", label: "Year (Newest)" },
        { value: "year-old", label: "Year (Oldest)" },
        { value: "journeys", label: "Most Journeys" },
      ]}
      w={{ base: "100%", sm: 200 }}
    />
  );
}
