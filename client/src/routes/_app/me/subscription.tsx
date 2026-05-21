import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  Badge,
  Button,
  Card,
  Container,
  List,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { ArrowLeft } from "lucide-react";
import { useEffect } from "react";
import { notify } from "@/integrations/mantine";

export const Route = createFileRoute("/_app/me/subscription")({
  staticData: { breadcrumb: "Subscription" },
  validateSearch: (search?): { reason?: string; from?: string } => ({
    reason: search?.reason as string | undefined,
    from: search?.from as string | undefined,
  }),
  component: SubscriptionPage,
});

function SubscriptionPage() {
  const navigate = useNavigate();
  const { reason, from } = Route.useSearch();

  useEffect(() => {
    if (reason === "car_limit_reached") {
      notify.warning({
        title: "Car Limit Reached",
        message:
          "You've reached the maximum number of cars allowed on your current plan. Please upgrade your subscription to add more cars.",
      });
    }
  }, []);
  return (
    <Container size="lg" py="xl">
      <Button
        component={Link}
        onClick={() => navigate({ to: from ?? "/dashboard" })}
        variant="subtle"
        leftSection={<ArrowLeft size={16} />}
        mb="xl"
      >
        Back
      </Button>

      <Stack gap="xl">
        <div>
          <Title order={1}>Subscription Plans</Title>
          <Text c="dimmed" mt="sm">
            Choose the plan that's right for you
          </Text>
        </div>

        <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="lg">
          <Card shadow="sm" padding="xl" radius="md" withBorder>
            <Stack gap="md">
              <div>
                <Badge size="lg" variant="light" mb="sm">
                  Free
                </Badge>
                <Title order={2}>Starter</Title>
                <Text size="xl" fw={700} mt="sm">
                  $0
                  <Text component="span" size="sm" c="dimmed">
                    /month
                  </Text>
                </Text>
              </div>

              <List spacing="sm">
                <List.Item>Up to 2 cars</List.Item>
                <List.Item>Unlimited journeys</List.Item>
                <List.Item>Basic image uploads</List.Item>
                <List.Item>Community access</List.Item>
              </List>

              <Button variant="light" disabled>
                Current Plan
              </Button>
            </Stack>
          </Card>

          <Card shadow="sm" padding="xl" radius="md" withBorder>
            <Stack gap="md">
              <div>
                <Badge size="lg" variant="filled" mb="sm">
                  Pro
                </Badge>
                <Title order={2}>Enthusiast</Title>
                <Text size="xl" fw={700} mt="sm">
                  $9.99
                  <Text component="span" size="sm" c="dimmed">
                    /month
                  </Text>
                </Text>
              </div>

              <List spacing="sm">
                <List.Item>Unlimited cars</List.Item>
                <List.Item>Unlimited journeys</List.Item>
                <List.Item>High-res image uploads</List.Item>
                <List.Item>Priority support</List.Item>
                <List.Item>Advanced analytics</List.Item>
                <List.Item>Custom badges</List.Item>
              </List>

              <Button>Coming Soon</Button>
            </Stack>
          </Card>
        </SimpleGrid>

        <Text size="sm" c="dimmed" ta="center">
          Subscription management and payment processing will be available soon.
        </Text>
      </Stack>
    </Container>
  );
}
