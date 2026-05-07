import { Group, Title, Text, Button, Stack } from "@mantine/core";
import { Plus } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { useCanAddCar } from "@/hooks/use-can-add-car";
import { openAddCarModal } from "@/lib/stores/cars-page-store";

export function CarsHeader() {
  const navigate = useNavigate();
  const { canAdd, isLoading } = useCanAddCar();

  const handleAddCar = () => {
    if (canAdd) {
      openAddCarModal();
    } else {
      navigate({ to: "/me/subscription", search: {} });
    }
  };

  return (
    <Group justify="space-between" align="flex-start" wrap="wrap" gap="md">
      <Stack gap={4}>
        <Title order={2}>My Cars</Title>
        <Text c="dimmed" size="sm">
          Manage your car collection
        </Text>
      </Stack>

      <Button
        leftSection={<Plus size={18} />}
        onClick={handleAddCar}
        loading={isLoading}
        style={{ minWidth: 140 }}
      >
        {canAdd ? "Add Car" : "Upgrade"}
      </Button>
    </Group>
  );
}
