import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { Container, Title } from "@mantine/core";
import { CarFormStepper } from "@/components/cars/car-form-stepper";
import { getCarLimitFn } from "@/lib/api/limits.server";

export const Route = createFileRoute("/_app/cars/new")({
  beforeLoad: async () => {
    const carLimits = await getCarLimitFn();
    if (!carLimits.canAdd) {
      throw redirect({
        to: "/subscription",
        search: { reason: "car_limit_reached", from: "/cars" },
      });
    }
  },
  component: AddCarPage,
});

function AddCarPage() {
  const navigate = useNavigate();

  return (
    <Container size="md" py="xl">
      <Title order={1} mb="xl">
        Add New Car
      </Title>
      <CarFormStepper
        onSuccess={() => navigate({ to: "/cars" })}
        onCancel={() => navigate({ to: "/cars" })}
      />
    </Container>
  );
}
