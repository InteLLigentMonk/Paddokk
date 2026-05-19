import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { Container, Title } from "@mantine/core";
import { CarFormStepper } from "@/components/cars/car-form-stepper";
import { getCarLimitFn } from "@/lib/api/limits.server";
import { currentUserQueryOptions } from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/cars/new")({
  staticData: { breadcrumb: "New car" },
  beforeLoad: async () => {
    const carLimits = await getCarLimitFn();
    if (!carLimits.canAdd) {
      throw redirect({
        to: "/me/subscription",
        search: { reason: "car_limit_reached", from: "/dashboard" },
      });
    }
  },
  loader: async ({ context: { queryClient } }) => {
    const me = await queryClient.ensureQueryData(currentUserQueryOptions());
    return { username: me.username };
  },
  component: AddCarPage,
});

function AddCarPage() {
  const navigate = useNavigate();
  const { username } = Route.useLoaderData();

  return (
    <Container size="md" py="xl">
      <Title order={1} mb="xl">
        Add New Car
      </Title>
      <CarFormStepper
        onSuccess={() =>
          navigate({ to: "/users/$username/cars", params: { username } })
        }
        onCancel={() =>
          navigate({ to: "/users/$username/cars", params: { username } })
        }
      />
    </Container>
  );
}
