import { ColorSwatch, Group, SimpleGrid, Text, Tooltip, UnstyledButton } from "@mantine/core";
import { Check } from "lucide-react";

export const OEM_SWATCHES = [
  { label: "Black", hex: "#0e0e0e" },
  { label: "White", hex: "#f3f3f3" },
  { label: "Silver", hex: "#bdc3c7" },
  { label: "Gunmetal", hex: "#3a3f44" },
  { label: "Midnight Blue", hex: "#0d1b2a" },
  { label: "British Racing Green", hex: "#0a3b1d" },
  { label: "Rosso Red", hex: "#b30000" },
  { label: "Sunburst Yellow", hex: "#ffc517" },
  { label: "Burnt Orange", hex: "#cc5500" },
  { label: "Champagne", hex: "#e6d3b3" },
  { label: "Pearl White", hex: "#eef2f7" },
  { label: "Matte Black", hex: "#1a1a1a" },
] as const;

export type SwatchLabel = (typeof OEM_SWATCHES)[number]["label"];

export function colorHexFromLabel(label: string | null | undefined): string | undefined {
  return OEM_SWATCHES.find((s) => s.label === label)?.hex;
}

interface CarColorSwatchInputProps {
  value: string | null | undefined;
  onChange: (label: string) => void;
}

export function CarColorSwatchInput({ value, onChange }: CarColorSwatchInputProps) {
  return (
    <div>
      <Text ff="monospace" tt="uppercase" fz={10} fw={700} c="dimmed" lts="0.12em" mb={8}>
        Color
      </Text>
      <SimpleGrid cols={6} spacing={6}>
        {OEM_SWATCHES.map((swatch) => {
          const selected = value === swatch.label;
          return (
            <Tooltip key={swatch.label} label={swatch.label} withArrow>
              <UnstyledButton onClick={() => onChange(swatch.label)} style={{ display: "flex" }}>
                <ColorSwatch
                  color={swatch.hex}
                  size={28}
                  style={{
                    outline: selected ? "2px solid var(--mantine-color-myColor-6)" : "2px solid transparent",
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
      {value && (
        <Group mt={6} gap={6} align="center">
          <ColorSwatch color={colorHexFromLabel(value) ?? "#888"} size={14} />
          <Text fz={12} c="dimmed">
            {value}
          </Text>
        </Group>
      )}
    </div>
  );
}
