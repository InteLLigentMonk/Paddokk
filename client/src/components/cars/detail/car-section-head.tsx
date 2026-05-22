import { Group, Text } from "@mantine/core";
import type { GroupProps } from "@mantine/core";
import type { ReactNode } from "react";

interface CarSectionHeadProps {
  kicker: string;
  title: string;
  count?: number | string | null;
  rightAction?: ReactNode;
  mb?: GroupProps["mb"];
}

export function CarSectionHead({
  kicker,
  title,
  count,
  rightAction,
  mb = "md",
}: CarSectionHeadProps) {
  return (
    <Group justify="space-between" align="flex-end" mb={mb}>
      <div>
        <Text
          ff="monospace"
          tt="uppercase"
          fz={10}
          fw={700}
          c="dimmed"
          lts="0.12em"
          mb={2}
        >
          {kicker}
        </Text>
        <Group gap="xs" align="baseline">
          <Text ff="Yapari" fz={22} fw={700} lh={1.1}>
            {title}
          </Text>
          {count != null && (
            <Text ff="monospace" fz={12} c="dimmed" fw={600}>
              {count}
            </Text>
          )}
        </Group>
      </div>
      {rightAction}
    </Group>
  );
}
