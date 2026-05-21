import { Paper, SimpleGrid, Skeleton, Stack, Text } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { browseCarsStatsQueryOptions } from "@/lib/api/cars.queries";
import { formatNumber } from "@/lib/utils/number-formatter";

interface CarsStatsCardProps {
  terms: string[];
}

interface StatItemProps {
  value: number | string | undefined;
  label: string;
  isLoading: boolean;
  lastStat?: boolean;
}

function StatItem({ value, label, isLoading, lastStat }: StatItemProps) {
  return (
    <Stack
      p="md"
      gap={2}
      style={{
        borderRight: lastStat
          ? ""
          : "1px solid var(--mantine-color-default-border",
      }}
    >
      <Text
        size="xs"
        c="dimmed"
        tt="uppercase"
        fw={500}
        style={{ letterSpacing: "0.05em" }}
      >
        {label}
      </Text>
      {isLoading ? (
        <Skeleton height={28} width={48} />
      ) : (
        <Text
          size="clamp(12px, 2.5vw, var(--mantine-font-size-d3))"
          ff="Yapari"
          fw={700}
          lh={1}
        >
          {formatNumber(value)}
        </Text>
      )}
    </Stack>
  );
}

export function CarsStatsCard({ terms }: CarsStatsCardProps) {
  const { data, isLoading } = useQuery(browseCarsStatsQueryOptions(terms));

  return (
    <Paper
      bdrs={0}
      style={{
        borderBlock: "1px solid var(--mantine-color-default-border)",
      }}
      // visibleFrom="sm"
    >
      <SimpleGrid cols={4} spacing={0}>
        <StatItem value={data?.cars} label="Cars" isLoading={isLoading} />
        <StatItem value={data?.makes} label="Makes" isLoading={isLoading} />
        <StatItem value={data?.owners} label="Owners" isLoading={isLoading} />
        <StatItem
          value={data?.journeys}
          label="Journeys"
          isLoading={isLoading}
          lastStat
        />
      </SimpleGrid>
    </Paper>
  );
}
