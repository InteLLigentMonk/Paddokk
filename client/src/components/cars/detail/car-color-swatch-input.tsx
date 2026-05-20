import {
  ColorSwatch,
  SimpleGrid,
  Text,
  Tooltip,
  UnstyledButton,
} from "@mantine/core";
import { Check } from "lucide-react";

export const CAR_COLORS = [
  // Row 1: Blacks & dark grays
  { label: "Gloss Black", hex: "#0a0a0a" },
  { label: "Matte Black", hex: "#1c1c1c" },
  { label: "Gunmetal", hex: "#3d4347" },
  { label: "Anthracite", hex: "#505050" },
  { label: "Titanium", hex: "#787878" },
  { label: "Platinum", hex: "#c0c0c0" },
  // Row 2: Silvers & whites
  { label: "Silver", hex: "#a8a9ad" },
  { label: "Pearl White", hex: "#eef0eb" },
  { label: "Alpine White", hex: "#f5f5f5" },
  { label: "Champagne", hex: "#e6d3b3" },
  { label: "Sahara", hex: "#c8a86e" },
  { label: "Bronze", hex: "#8c6a3f" },
  // Row 3: Blues
  { label: "Ice Blue", hex: "#b0cee0" },
  { label: "Sky Blue", hex: "#5b9fd6" },
  { label: "Cobalt", hex: "#1a6bb5" },
  { label: "Sapphire", hex: "#0a3d7a" },
  { label: "Midnight Blue", hex: "#0d1b2a" },
  { label: "Petrol", hex: "#005f73" },
  // Row 4: Warm colors
  { label: "Rosso Red", hex: "#e00000" },
  { label: "Burgundy", hex: "#6e001e" },
  { label: "Racing Green", hex: "#0a5c1e" },
  { label: "Burnt Orange", hex: "#cc5500" },
  { label: "Sunburst", hex: "#ffd500" },
  { label: "Lime", hex: "#6ab82f" },
] as const;

export function colorLabelFromHex(
  hex: string | null | undefined,
): string | undefined {
  return CAR_COLORS.find((c) => c.hex === hex)?.label;
}

interface CarColorSwatchInputProps {
  value: string | null | undefined;
  onChange: (hex: string) => void;
}

export function CarColorSwatchInput({
  value,
  onChange,
}: CarColorSwatchInputProps) {
  return (
    <div>
      <Text
        ff="monospace"
        tt="uppercase"
        fz={10}
        fw={700}
        c="dimmed"
        lts="0.12em"
        mb={8}
      >
        Color
      </Text>
      <SimpleGrid w="fit-content" cols={6} spacing={6}>
        {CAR_COLORS.map((car) => {
          const selected = value === car.hex;
          return (
            <Tooltip key={car.hex} label={car.label} withArrow>
              <UnstyledButton
                onClick={() => onChange(car.hex)}
                style={{ display: "flex" }}
              >
                <ColorSwatch
                  color={car.hex}
                  size={28}
                  style={{
                    outline: selected
                      ? "2px solid var(--mantine-color-myColor-6)"
                      : "2px solid transparent",
                    outlineOffset: 2,
                    cursor: "pointer",
                  }}
                >
                  {selected && <Check size={14} color="white" />}
                </ColorSwatch>
              </UnstyledButton>
            </Tooltip>
          );
        })}
      </SimpleGrid>
    </div>
  );
}
