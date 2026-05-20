import { Select } from "@mantine/core";

const DRIVE_OPTIONS = [
  { value: "0", label: "FWD" },
  { value: "1", label: "RWD" },
  { value: "2", label: "AWD" },
  { value: "3", label: "4WD" },
];

export const DRIVE_LABELS: Record<number, string> = {
  0: "FWD",
  1: "RWD",
  2: "AWD",
  3: "4WD",
};

interface CarDriveSelectProps {
  value: number | null | undefined;
  onChange: (value: number | null) => void;
  label?: string;
}

export function CarDriveSelect({ value, onChange, label }: CarDriveSelectProps) {
  return (
    <Select
      label={label}
      placeholder="Select drive type"
      data={DRIVE_OPTIONS}
      value={value != null ? String(value) : null}
      onChange={(v) => onChange(v != null ? Number(v) : null)}
      clearable
    />
  );
}
