import { useState } from "react";
import {
  Card,
  ColorSwatch,
  Text,
  Stack,
  Group,
  Divider,
  ActionIcon,
  TextInput,
  NumberInput,
  Button,
} from "@mantine/core";
import {
  Edit,
  Check,
  X,
  MapPin,
  Compass,
  Clock,
  Gauge,
  PaletteIcon,
} from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto } from "@/generated/api/schemas";
import { updateUserCarFn } from "@/lib/api/user-cars.server";
import {
  CarColorSwatchInput,
  colorLabelFromHex,
} from "./car-color-swatch-input";

interface VitalsRowProps {
  label: string;
  value: string | null | undefined;
  icon?: React.ReactNode;
}

function VitalsRow({ label, value, icon }: VitalsRowProps) {
  return (
    <Group gap="md">
      {icon && icon}
      <Stack gap={0}>
        <Text
          ff="monospace"
          tt="uppercase"
          fz={10}
          fw={700}
          c="dimmed"
          lts="0.1em"
        >
          {label}
        </Text>
        <Text c={value ? undefined : "dimmed"}>{value ?? "—"}</Text>
      </Stack>
    </Group>
  );
}

interface CarVitalsCardProps {
  car: UserCarDto;
}

export function CarVitalsCard({ car }: CarVitalsCardProps) {
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [region, setRegion] = useState(car.region ?? "");
  const [color, setColor] = useState(car.color ?? null);
  const [odometerKm, setOdometerKm] = useState<number | string>(
    car.odometerKm != null ? Number(car.odometerKm) : "",
  );

  const joinedAt = new Date(car.createdAt).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  const odoDisplay =
    car.odometerKm != null
      ? `${Number(car.odometerKm).toLocaleString()} km`
      : null;

  const handleSave = async () => {
    setIsSaving(true);
    try {
      await updateUserCarFn({
        data: {
          carId: Number(car.id),
          region: region || null,
          color: color,
          odometerKm: odometerKm !== "" ? Number(odometerKm) : null,
        },
      });
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
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
    setColor(car.color ?? null);
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
          <ActionIcon
            variant="subtle"
            size="sm"
            onClick={() => setIsEditing(true)}
          >
            <Edit size={14} />
          </ActionIcon>
        )}
      </Group>
      <Divider mb="sm" />

      {isEditing ? (
        <Stack gap="sm">
          <CarColorSwatchInput value={color} onChange={setColor} />
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
            <Button
              size="xs"
              onClick={handleSave}
              loading={isSaving}
              leftSection={<Check size={12} />}
            >
              Save
            </Button>
            <Button
              size="xs"
              variant="subtle"
              onClick={handleCancel}
              disabled={isSaving}
              leftSection={<X size={12} />}
            >
              Cancel
            </Button>
          </Group>
        </Stack>
      ) : (
        <Stack gap={8}>
          {car.color && (
            <Group gap="md">
              <PaletteIcon size={14} />
              <Stack gap={0}>
                <Text
                  ff="monospace"
                  tt="uppercase"
                  fz={10}
                  fw={700}
                  c="dimmed"
                  lts="0.1em"
                >
                  Color
                </Text>
                <Group gap={6} justify="flex-end">
                  <ColorSwatch color={car.color ?? "#888"} size={14} />
                  <Text fz={13} fw={500} ta="right">
                    {colorLabelFromHex(car.color) ?? car.color}
                  </Text>
                </Group>
              </Stack>
            </Group>
          )}
          <VitalsRow
            label="Based"
            value={car.region}
            icon={<MapPin size={14} />}
          />
          <VitalsRow
            label="Odometer"
            value={odoDisplay}
            icon={<Gauge size={14} />}
          />
          <VitalsRow
            label="On Paddokk since"
            value={joinedAt}
            icon={<Clock size={14} />}
          />
          <VitalsRow
            label="Journeys"
            value={String(Number(car.journeyCount))}
            icon={<Compass size={14} />}
          />
        </Stack>
      )}
    </Card>
  );
}
