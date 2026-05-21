import { Group, Select, TagsInput } from "@mantine/core";
import { useNavigate } from "@tanstack/react-router";
import type { JourneySortKey } from "@/lib/api/journeys";

const SORT_OPTIONS: Array<{ value: JourneySortKey; label: string }> = [
  { value: "RecentActivity", label: "Recent activity" },
  { value: "Newest", label: "Newest" },
  { value: "MostLiked", label: "Most liked" },
  { value: "MostSubscribed", label: "Most subscribed" },
  { value: "RecentlyCompleted", label: "Recently completed" },
];

interface JourneysFilterBarProps {
  terms: Array<string>;
  sort: JourneySortKey | undefined;
}

export function JourneysFilterBar({ terms, sort }: JourneysFilterBarProps) {
  const navigate = useNavigate();

  function updateSearch(newTerms: Array<string>, newSort: JourneySortKey | undefined) {
    navigate({
      to: "/journeys",
      search: {
        q: newTerms.length > 0 ? newTerms : undefined,
        sort: newSort,
      },
    });
  }

  const effectiveSortKey: JourneySortKey =
    sort ?? (terms.length > 0 ? "RecentActivity" : "Newest");

  return (
    <Group
      gap="sm"
      wrap="nowrap"
      bg="light-dark(var(--mantine-color-white), var(--mantine-color-dark-7))"
      align="flex-start"
      style={{ position: "sticky", top: 0, zIndex: 100 }}
    >
      <TagsInput
        placeholder="Search by title, car, make or model..."
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
        onChange={(val) => updateSearch(terms, val ? val : undefined)}
        w={200}
        allowDeselect={false}
      />
    </Group>
  );
}
