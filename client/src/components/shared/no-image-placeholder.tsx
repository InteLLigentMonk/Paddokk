import { Box, Stack, Text } from "@mantine/core";
import { Car, ImageOff, MapPinned } from "lucide-react";
import classes from "./no-image-placeholder.module.css";

export type PlaceholderVariant = "car" | "journey" | "photo";

interface NoImagePlaceholderProps {
  variant?: PlaceholderVariant;
  label?: string;
}

const VARIANTS = {
  car: { Icon: Car, defaultLabel: "No image" },
  journey: { Icon: MapPinned, defaultLabel: "No cover" },
  photo: { Icon: ImageOff, defaultLabel: "No image" },
} as const;

export function NoImagePlaceholder({
  variant = "photo",
  label,
}: NoImagePlaceholderProps) {
  const { Icon, defaultLabel } = VARIANTS[variant];

  return (
    <Box className={classes.root} role="img" aria-label={label ?? defaultLabel}>
      <Stack className={classes.content} gap={6} align="center">
        <Icon className={classes.icon} strokeWidth={1.25} aria-hidden />
        <Text className={classes.label} ff="monospace" tt="uppercase">
          {label ?? defaultLabel}
        </Text>
      </Stack>
    </Box>
  );
}
