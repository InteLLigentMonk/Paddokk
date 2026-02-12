import { Link } from "@tanstack/react-router";
import { Button, Container, Group, Stack, Text, Title } from "@mantine/core";
import { SectionWrapper } from "../layout/section-wrapper";

export function HeroSection() {
  return (
    <SectionWrapper variant="default">
      <Container size="md">
        <Stack align="center" gap="xl" ta="center">
          {/* Heading */}
          <Stack gap="md">
            <Title
              order={1}
              size="h1"
              fw={800}
              style={{
                fontSize: "clamp(2rem, 5vw, 3.5rem)",
                lineHeight: 1.2,
              }}
            >
              The Social Platform for Car Enthusiasts
            </Title>
            <Text size="lg" c="dimmed" maw={600} mx="auto">
              Document your car journeys, share your builds, and connect with
              fellow car enthusiasts. Paddokk bridges the gap between forums and
              modern social platforms.
            </Text>
          </Stack>

          {/* CTA Buttons */}
          <Group justify="center" gap="md">
            <Button component={Link} to="/signup" size="lg">
              Get Started
            </Button>
            <Button component={Link} to="/login" variant="default" size="lg">
              Sign In
            </Button>
          </Group>

          {/* Optional: Add a subtle note */}
          <Text size="sm" c="dimmed">
            Free to join • No credit card required
          </Text>
        </Stack>
      </Container>
    </SectionWrapper>
  );
}
