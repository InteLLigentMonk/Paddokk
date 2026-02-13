import {
  ActionIcon,
  Box,
  useComputedColorScheme,
  useMantineColorScheme,
} from "@mantine/core";
import { Moon, Sun } from "lucide-react";

const iconProps = { size: 18, strokeWidth: 1.5 } as const;

export function ColorSchemeToggle() {
  const { setColorScheme } = useMantineColorScheme();
  const computedColorScheme = useComputedColorScheme("light", {
    getInitialValueInEffect: true,
  });

  return (
    <ActionIcon
      variant="default"
      size="lg"
      aria-label="Toggle color scheme"
      onClick={() =>
        setColorScheme(computedColorScheme === "light" ? "dark" : "light")
      }
    >
      <Box component="span" darkHidden>
        <Sun {...iconProps} />
      </Box>
      <Box component="span" lightHidden>
        <Moon {...iconProps} />
      </Box>
    </ActionIcon>
  );
}
