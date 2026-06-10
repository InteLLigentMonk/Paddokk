import { Container, Divider, Stack, Text, Title } from "@mantine/core";
import type { ReactNode } from "react";
import { LEGAL_VERSION_LABEL } from "@/lib/legal/legal-version";

/**
 * Shared chrome for a legal page: a constrained reading column, the document
 * title with an intro line, the authored sections, and the "Last updated"
 * version stamp sourced from the single legal-version constant.
 */
export function LegalDocument({
  title,
  intro,
  children,
}: {
  title: string;
  intro: string;
  children: ReactNode;
}) {
  return (
    <Container size="sm" py={{ base: "xl", sm: 48 }}>
      <Stack gap="xl">
        <Stack gap="xs">
          <Title order={1}>{title}</Title>
          <Text c="dimmed">{intro}</Text>
        </Stack>

        {children}

        <Divider />

        <Text fz="sm" c="dimmed">
          Last updated: {LEGAL_VERSION_LABEL}
        </Text>
      </Stack>
    </Container>
  );
}

/** A titled section within a legal document. */
export function LegalSection({
  heading,
  children,
}: {
  heading: string;
  children: ReactNode;
}) {
  return (
    <Stack gap="sm">
      <Title order={2} fz="h3">
        {heading}
      </Title>
      {children}
    </Stack>
  );
}
