import { Badge, Container, Stack, Text, Timeline, Title } from "@mantine/core";
import { SectionWrapper } from "../layout/section-wrapper";

interface RoadmapItem {
  phase: string;
  title: string;
  description: string;
  status: "complete" | "active" | "planned";
}

const roadmapItems: Array<RoadmapItem> = [
  {
    phase: "Q1 2026",
    title: "Platform Launch",
    description:
      "Core features including journey tracking, photo uploads, and community forums.",
    status: "active",
  },
  {
    phase: "Q2 2026",
    title: "Mobile Apps",
    description:
      "Native iOS and Android apps for on-the-go journey updates and community engagement.",
    status: "planned",
  },
  {
    phase: "Q3 2026",
    title: "Marketplace",
    description:
      "Buy and sell parts, find services, and connect with local shops and mechanics.",
    status: "planned",
  },
  {
    phase: "Q4 2026",
    title: "Events Platform",
    description:
      "Discover and organize car meets, track days, and road trips with integrated RSVP.",
    status: "planned",
  },
];

const statusColors = {
  complete: "green",
  active: "myColor",
  planned: "gray",
} as const;

const statusLabels = {
  complete: "Completed",
  active: "In Progress",
  planned: "Planned",
} as const;

export function RoadmapSection() {
  return (
    <SectionWrapper variant="muted" id="roadmap">
      <Container size="md">
        <Stack gap="xl">
          {/* Section Header */}
          <Stack gap="sm" ta="center">
            <Title order={2} size="h2" fw={700}>
              Product Roadmap
            </Title>
            <Text size="lg" c="dimmed" maw={600} mx="auto">
              We're building the future of automotive social networking. Here's
              what's coming next.
            </Text>
          </Stack>

          {/* Timeline */}
          <Timeline
            active={roadmapItems.findIndex((item) => item.status === "active")}
            bulletSize={24}
            lineWidth={2}
            mt="xl"
          >
            {roadmapItems.map((item) => (
              <Timeline.Item
                key={item.phase}
                title={
                  <Stack gap="xs">
                    <Text fw={600} size="lg">
                      {item.title}
                    </Text>
                    <Badge
                      color={statusColors[item.status]}
                      variant="light"
                      size="sm"
                    >
                      {statusLabels[item.status]}
                    </Badge>
                  </Stack>
                }
              >
                <Text size="sm" c="dimmed" mt="xs">
                  {item.phase}
                </Text>
                <Text size="sm" mt="xs">
                  {item.description}
                </Text>
              </Timeline.Item>
            ))}
          </Timeline>
        </Stack>
      </Container>
    </SectionWrapper>
  );
}
