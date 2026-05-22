import { useState } from "react";
import {
  ActionIcon,
  Box,
  Button,
  Group,
  NumberInput,
  SimpleGrid,
  Text,
  TextInput,
} from "@mantine/core";
import { Check, Edit, X } from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";
import { eraFromYear } from "./era";
import { CarDriveSelect, DRIVE_LABELS } from "./car-drive-select";

import type { UserCarDto } from "@/generated/api/schemas";
import { updateUserCarFn } from "@/lib/api/user-cars";
import { useNotifications } from "@/integrations/mantine";
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
        style={{ overflowWrap: "break-word" }}
      >
        {value ?? "â€”"}
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
    car.drive != null ? car.drive : null,
  );
  const [engine, setEngine] = useState(car.engine ?? "");
  const [odometerKm, setOdometerKm] = useState<number | string>(
    car.odometerKm != null ? Number(car.odometerKm) : "",
  );

  const era = eraFromYear(car.year);
  const driveLabel = car.drive != null ? DRIVE_LABELS[car.drive] : null;
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
    setDrive(car.drive != null ? car.drive : null);
    setEngine(car.engine ?? "");
    setOdometerKm(car.odometerKm != null ? Number(car.odometerKm) : "");
    setIsEditing(false);
  };

  if (isEditing) {
    return (
      <Box
        p="md"
        style={{
          borderBottom: "1px solid var(--mantine-color-default-border)",
        }}
      >
        <SimpleGrid cols={{ base: 2, md: 5 }} spacing="md">
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
          <Group gap="xs" align="flex-end" wrap="nowrap" miw="max-content">
            <Button
              size="sm"
              onClick={handleSave}
              loading={isSaving}
              leftSection={<Check size={13} />}
            >
              Save
            </Button>
            <Button
              size="sm"
              variant="subtle"
              onClick={handleCancel}
              disabled={isSaving}
              leftSection={<X size={13} />}
            >
              Cancel
            </Button>
          </Group>
        </SimpleGrid>
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
