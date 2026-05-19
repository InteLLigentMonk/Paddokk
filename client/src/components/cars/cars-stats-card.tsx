import { Group, Paper, Skeleton, Stack, Text } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { browseCarsStatsQueryOptions } from "@/lib/api/cars.queries";

interface CarsStatsCardProps {
  terms: string[];
}

interface StatItemProps {
  value: number | string | undefined;
  label: string;
  isLoading: boolean;
}

function StatItem({ value, label, isLoading }: StatItemProps) {
  return (
    <Stack gap={2} align="center">
      {isLoading ? (
        <Skeleton height={28} width={48} />
      ) : (
        <Text size="xl" fw={700} lh={1}>
          {Number(value ?? 0).toLocaleString("sv-SE")}
        </Text>
      )}
      <Text
        size="xs"
        c="dimmed"
        tt="uppercase"
        fw={500}
        style={{ letterSpacing: "0.05em" }}
      >
        {label}
      </Text>
    </Stack>
  );
}

export function CarsStatsCard({ terms }: CarsStatsCardProps) {
  const { data, isLoading } = useQuery(browseCarsStatsQueryOptions(terms));

  return (
    <Paper withBorder radius="md" p="md">
      <Group justify="space-around" wrap="wrap" gap="md">
        <StatItem value={data?.cars} label="Cars" isLoading={isLoading} />
        <StatItem value={data?.makes} label="Makes" isLoading={isLoading} />
        <StatItem value={data?.owners} label="Owners" isLoading={isLoading} />
        <StatItem
          value={data?.journeys}
          label="Journeys"
          isLoading={isLoading}
        />
      </Group>
    </Paper>
  );
}
