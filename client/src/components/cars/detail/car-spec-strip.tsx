import { useState } from "react";
import {
  Box,
  Group,
  Text,
  ActionIcon,
  TextInput,
  NumberInput,
  Button,
  SimpleGrid,
} from "@mantine/core";
import { Edit, Check, X } from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto } from "@/generated/api/schemas";
import { updateUserCarFn } from "@/lib/api/user-cars.server";
import { eraFromYear } from "./era";
import { DRIVE_LABELS } from "./car-drive-select";
import { CarDriveSelect } from "./car-drive-select";
import { formatNumber } from "@/lib/utils/number-formatter";

interface StripCellProps {
  label: string;
  value: string | null | undefined;
}

function StripCell({ label, value }: StripCellProps) {
  return (
    <Box
      px={20}
      py={14}
      style={{
        borderBottom: "1px solid var(--mantine-color-default-border)",
        borderRight: "1px solid var(--mantine-color-default-border)",
      }}
    >
      <Text
        ff="monospace"
        tt="uppercase"
        fz={10}
        fw={700}
        c="dimmed"
        lts="0.12em"
        mb={4}
      >
        {label}
      </Text>
      <Text
        ff="Yapari"
        fz={18}
        fw={400}
        lh={1.1}
        c={value ? undefined : "dimmed"}
      >
        {value ?? "—"}
      </Text>
    </Box>
  );
}

interface CarSpecStripProps {
  car: UserCarDto;
}

export function CarSpecStrip({ car }: CarSpecStripProps) {
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [region, setRegion] = useState(car.region ?? "");
  const [drive, setDrive] = useState<number | null>(
    car.drive != null ? (car.drive as number) : null,
  );
  const [engine, setEngine] = useState(car.engine ?? "");
  const [odometerKm, setOdometerKm] = useState<number | string>(
    car.odometerKm != null ? Number(car.odometerKm) : "",
  );

  const era = eraFromYear(car.year);
  const driveLabel =
    car.drive != null ? DRIVE_LABELS[car.drive as number] : null;
  const odoDisplay = `${formatNumber(car.odometerKm)} km`;

  const handleSave = async () => {
    setIsSaving(true);
    try {
      await updateUserCarFn({
        data: {
          carId: Number(car.id),
          region: region || null,
          drive: drive,
          engine: engine || null,
          odometerKm: odometerKm !== "" ? Number(odometerKm) : null,
        },
      });
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
      notifications.success({ message: "Specs updated" });
      setIsEditing(false);
    } catch {
      notifications.error({ message: "Failed to save specs" });
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setRegion(car.region ?? "");
    setDrive(car.drive != null ? (car.drive as number) : null);
    setEngine(car.engine ?? "");
    setOdometerKm(car.odometerKm != null ? Number(car.odometerKm) : "");
    setIsEditing(false);
  };

  if (isEditing) {
    return (
      <Box
        px={{ base: 20, md: 36 }}
        py={16}
        style={{
          background:
            "light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-7))",
          borderBottom:
            "1px solid light-dark(var(--mantine-color-gray-2), var(--mantine-color-dark-5))",
        }}
      >
        <SimpleGrid cols={{ base: 2, sm: 4 }} spacing="sm" mb={12}>
          <TextInput
            label="Region"
            value={region}
            onChange={(e) => setRegion(e.currentTarget.value)}
            placeholder="e.g. Japan"
            size="sm"
          />
          <CarDriveSelect value={drive} onChange={setDrive} label="Drive" />
          <TextInput
            label="Engine"
            value={engine}
            onChange={(e) => setEngine(e.currentTarget.value)}
            placeholder="e.g. 2JZ-GTE"
            size="sm"
          />
          <NumberInput
            label="Odometer (km)"
            value={odometerKm}
            onChange={setOdometerKm}
            min={0}
            placeholder="e.g. 145000"
            size="sm"
          />
        </SimpleGrid>
        <Group gap="xs">
          <Button
            size="xs"
            onClick={handleSave}
            loading={isSaving}
            leftSection={<Check size={13} />}
          >
            Save
          </Button>
          <Button
            size="xs"
            variant="subtle"
            onClick={handleCancel}
            disabled={isSaving}
            leftSection={<X size={13} />}
          >
            Cancel
          </Button>
        </Group>
      </Box>
    );
  }

  return (
    <Box
      style={{
        overflowX: "auto",
      }}
    >
      <SimpleGrid
        cols={{ base: 2, sm: 3, lg: 6 }}
        spacing={0}
        style={{ position: "relative", minWidth: "min-content" }}
      >
        <StripCell
          label="Year"
          value={car.year != null ? String(car.year) : null}
        />
        <StripCell label="Era" value={era} />
        <StripCell label="Region" value={car.region} />
        <StripCell label="Drive" value={driveLabel} />
        <StripCell label="Engine" value={car.engine} />
        <StripCell label="Odometer" value={odoDisplay} />

        {car.isOwner && (
          <Box style={{ position: "absolute", right: 8, top: 8 }}>
            <ActionIcon
              variant="subtle"
              size="sm"
              onClick={() => setIsEditing(true)}
            >
              <Edit size={14} />
            </ActionIcon>
          </Box>
        )}
      </SimpleGrid>
    </Box>
  );
}
