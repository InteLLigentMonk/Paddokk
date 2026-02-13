import { Link } from "@tanstack/react-router";
import {
  Badge,
  Button,
  Card,
  Container,
  Group,
  List,
  SimpleGrid,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { Check } from "lucide-react";
import { SectionWrapper } from "../layout/section-wrapper";

interface PricingPlan {
  name: string;
  price: string;
  description: string;
  features: Array<string>;
  popular?: boolean;
  cta: string;
  ctaVariant?: "filled" | "default";
}

const plans: Array<PricingPlan> = [
  {
    name: "Free",
    price: "$0",
    description: "Perfect for getting started",
    features: [
      "Up to 3 active journeys",
      "Basic photo uploads",
      "Community access",
      "Public profile",
    ],
    cta: "Get Started",
    ctaVariant: "default",
  },
  {
    name: "Pro",
    price: "$9",
    description: "For serious enthusiasts",
    features: [
      "Unlimited journeys",
      "High-res photo & video uploads",
      "Advanced analytics",
      "Priority support",
      "Custom profile themes",
      "Ad-free experience",
    ],
    popular: true,
    cta: "Start Free Trial",
    ctaVariant: "filled",
  },
  {
    name: "Club",
    price: "$29",
    description: "For car clubs & teams",
    features: [
      "Everything in Pro",
      "Up to 50 members",
      "Club branding",
      "Event management",
      "Private forums",
      "Dedicated support",
    ],
    cta: "Contact Sales",
    ctaVariant: "default",
  },
];

export function PricingSection() {
  return (
    <SectionWrapper variant="default" id="pricing">
      <Container size="lg">
        <Stack gap="xl">
          {/* Section Header */}
          <Stack gap="sm" ta="center">
            <Title order={2} size="h2" fw={700}>
              Simple, Transparent Pricing
            </Title>
            <Text size="lg" c="dimmed" maw={600} mx="auto">
              Choose the plan that fits your needs. All plans include core
              features with no hidden fees.
            </Text>
          </Stack>

          {/* Pricing Cards */}
          <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg" mt="xl">
            {plans.map((plan) => (
              <Card
                key={plan.name}
                shadow="sm"
                padding="lg"
                radius="md"
                withBorder
                style={{
                  position: "relative",
                  ...(plan.popular && {
                    borderColor: "var(--mantine-primary-color-filled)",
                    borderWidth: 2,
                  }),
                }}
              >
                {plan.popular && (
                  <Badge
                    color="myColor"
                    variant="filled"
                    style={{
                      position: "absolute",
                      top: -12,
                      right: 20,
                    }}
                  >
                    Most Popular
                  </Badge>
                )}

                <Stack gap="md">
                  {/* Plan Header */}
                  <Stack gap="xs">
                    <Text fw={600} size="xl">
                      {plan.name}
                    </Text>
                    <Group gap="xs" align="baseline">
                      <Text fw={700} size="2.5rem" lh={1}>
                        {plan.price}
                      </Text>
                      <Text c="dimmed">/month</Text>
                    </Group>
                    <Text size="sm" c="dimmed">
                      {plan.description}
                    </Text>
                  </Stack>

                  {/* Features List */}
                  <List
                    spacing="sm"
                    size="sm"
                    icon={
                      <Check
                        size={16}
                        style={{ color: "var(--mantine-primary-color-filled)" }}
                      />
                    }
                  >
                    {plan.features.map((feature) => (
                      <List.Item key={feature}>{feature}</List.Item>
                    ))}
                  </List>

                  {/* CTA Button */}
                  <Button
                    component={Link}
                    to="/signup"
                    variant={plan.ctaVariant}
                    fullWidth
                    mt="auto"
                  >
                    {plan.cta}
                  </Button>
                </Stack>
              </Card>
            ))}
          </SimpleGrid>
        </Stack>
      </Container>
    </SectionWrapper>
  );
}
