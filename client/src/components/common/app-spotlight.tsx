import { Spotlight } from "@mantine/spotlight";
import { Search } from "lucide-react";
import { useState } from "react";

const iconProps = { size: 18, strokeWidth: 1.5 } as const;

export function AppSpotlight() {
  const [query, setQuery] = useState("");

  // TODO: Replace with actual search results
  const actions = [
    {
      id: "home",
      label: "Home",
      description: "Go to home page",
      onClick: () => console.log("Navigate to home"),
    },
    {
      id: "dashboard",
      label: "Dashboard",
      description: "Go to dashboard",
      onClick: () => console.log("Navigate to dashboard"),
    },
    {
      id: "settings",
      label: "Settings",
      description: "Open settings",
      onClick: () => console.log("Navigate to settings"),
    },
  ];

  return (
    <Spotlight
      actions={actions}
      nothingFound="Nothing found..."
      highlightQuery
      searchProps={{
        leftSection: <Search {...iconProps} />,
        placeholder: "Search...",
      }}
      shortcut="mod+k"
      query={query}
      onQueryChange={setQuery}
    />
  );
}
