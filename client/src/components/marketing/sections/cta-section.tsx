import { Button, Container, Group, Stack, Text, Title } from "@mantine/core";
import { SectionWrapper } from "../layout/section-wrapper";
import { openLogin, openSignup } from "@/lib/stores/auth-modal-store";

export function CtaSection() {
  return (
    <SectionWrapper variant="primary">
      <Container size="md">
        <Stack align="center" gap="xl" ta="center">
          <Stack gap="md">
            <Title order={2} size="h2" fw={700}>
              Ready to Start Your Journey?
            </Title>
            <Text size="lg" c="dimmed" maw={600} mx="auto">
              Join thousands of car enthusiasts already documenting their builds
              and connecting with the community.
            </Text>
          </Stack>

          <Group justify="center" gap="md">
            <Button onClick={openSignup} size="lg" variant="white">
              Get Started Free
            </Button>
            <Button onClick={openLogin} size="lg" variant="default">
              Sign In
            </Button>
          </Group>
        </Stack>
      </Container>
    </SectionWrapper>
  );
}
