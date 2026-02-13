import { Container, SimpleGrid, Stack, Text, Title } from "@mantine/core";
import { BookOpen, Camera, Car, Users } from "lucide-react";
import { SectionWrapper } from "../layout/section-wrapper";

interface Feature {
  icon: React.ReactNode;
  title: string;
  description: string;
}

const features: Array<Feature> = [
  {
    icon: <Car size={32} />,
    title: "Journey Tracking",
    description:
      "Document your car builds, modifications, and road trips. Track progress with photos, notes, and timeline views.",
  },
  {
    icon: <Users size={32} />,
    title: "Community Driven",
    description:
      "Connect with fellow enthusiasts, join groups, and participate in discussions. Forums meet modern social.",
  },
  {
    icon: <Camera size={32} />,
    title: "Rich Media Sharing",
    description:
      "Share high-quality photos and videos of your rides. Showcase your builds with stunning visuals.",
  },
  {
    icon: <BookOpen size={32} />,
    title: "Knowledge Base",
    description:
      "Learn from experienced builders. Access guides, tutorials, and community wisdom in one place.",
  },
];

export function FeaturesSection() {
  return (
    <SectionWrapper variant="muted" id="features">
      <Container size="lg">
        <Stack gap="xl">
          {/* Section Header */}
          <Stack gap="sm" ta="center">
            <Title order={2} size="h2" fw={700}>
              Everything You Need
            </Title>
            <Text size="lg" c="dimmed" maw={600} mx="auto">
              Built for car enthusiasts, by car enthusiasts. All the tools you
              need to document and share your automotive journey.
            </Text>
          </Stack>

          {/* Features Grid */}
          <SimpleGrid cols={{ base: 1, sm: 2, md: 4 }} spacing="lg" mt="xl">
            {features.map((feature) => (
              <Stack key={feature.title} gap="md">
                <div style={{ color: "var(--mantine-primary-color-filled)" }}>
                  {feature.icon}
                </div>
                <Stack gap="xs">
                  <Text fw={600} size="lg">
                    {feature.title}
                  </Text>
                  <Text size="sm" c="dimmed">
                    {feature.description}
                  </Text>
                </Stack>
              </Stack>
            ))}
          </SimpleGrid>
        </Stack>
      </Container>
    </SectionWrapper>
  );
}
