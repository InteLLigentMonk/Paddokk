import { useState } from "react";
import {
  Card,
  Text,
  Stack,
  Group,
  Divider,
  ActionIcon,
  TextInput,
  NumberInput,
  Button,
} from "@mantine/core";
import { Edit, Check, X } from "lucide-react";
import { useRouter } from "@tanstack/react-router";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto } from "@/generated/api/schemas";
import { updateUserCarFn } from "@/lib/api/user-cars.server";

interface VitalsRowProps {
  label: string;
  value: string | null | undefined;
}

function VitalsRow({ label, value }: VitalsRowProps) {
  return (
    <Group justify="space-between" gap="xs">
      <Text ff="monospace" tt="uppercase" fz={10} fw={700} c="dimmed" lts="0.1em">
        {label}
      </Text>
      <Text fz={13} fw={500} c={value ? undefined : "dimmed"} ta="right" style={{ flex: 1, textAlign: "right" }}>
        {value ?? "—"}
      </Text>
    </Group>
  );
}

interface CarVitalsCardProps {
  car: UserCarDto;
}

export function CarVitalsCard({ car }: CarVitalsCardProps) {
  const router = useRouter();
  const notifications = useNotifications();
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [region, setRegion] = useState(car.region ?? "");
  const [odometerKm, setOdometerKm] = useState<number | string>(
    car.odometerKm != null ? Number(car.odometerKm) : "",
  );

  const joinedAt = new Date(car.createdAt).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  const odoDisplay = car.odometerKm != null ? `${Number(car.odometerKm).toLocaleString()} km` : null;

  const handleSave = async () => {
    setIsSaving(true);
    try {
      await updateUserCarFn({
        data: {
          carId: Number(car.id),
          region: region || null,
          odometerKm: odometerKm !== "" ? Number(odometerKm) : null,
        },
      });
      await router.invalidate();
      notifications.success({ message: "Vitals updated" });
      setIsEditing(false);
    } catch {
      notifications.error({ message: "Failed to save" });
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setRegion(car.region ?? "");
    setOdometerKm(car.odometerKm != null ? Number(car.odometerKm) : "");
    setIsEditing(false);
  };

  return (
    <Card withBorder radius="md" padding="md">
      <Group justify="space-between" mb="xs">
        <Text ff="monospace" tt="uppercase" fz={11} fw={700} lts="0.12em">
          Vitals
        </Text>
        {car.isOwner && !isEditing && (
          <ActionIcon variant="subtle" size="sm" onClick={() => setIsEditing(true)}>
            <Edit size={14} />
          </ActionIcon>
        )}
      </Group>
      <Divider mb="sm" />

      {isEditing ? (
        <Stack gap="sm">
          <TextInput
            label="Based (region)"
            value={region}
            onChange={(e) => setRegion(e.currentTarget.value)}
            placeholder="e.g. Japan"
            size="xs"
          />
          <NumberInput
            label="Odometer (km)"
            value={odometerKm}
            onChange={setOdometerKm}
            min={0}
            placeholder="e.g. 145000"
            size="xs"
          />
          <Group gap="xs">
            <Button size="xs" onClick={handleSave} loading={isSaving} leftSection={<Check size={12} />}>
              Save
            </Button>
            <Button size="xs" variant="subtle" onClick={handleCancel} disabled={isSaving} leftSection={<X size={12} />}>
              Cancel
            </Button>
          </Group>
        </Stack>
      ) : (
        <Stack gap={8}>
          <VitalsRow label="Based" value={car.region} />
          <VitalsRow label="Odometer" value={odoDisplay} />
          <VitalsRow label="On Paddokk" value={joinedAt} />
          <VitalsRow label="Journeys" value={String(Number(car.journeyCount))} />
        </Stack>
      )}
    </Card>
  );
}
