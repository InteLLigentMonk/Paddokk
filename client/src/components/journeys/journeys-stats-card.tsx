import { Paper, SimpleGrid, Skeleton, Stack, Text } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { browseJourneysStatsQueryOptions } from "@/lib/api/journeys.queries";
import { formatNumber } from "@/lib/utils/number-formatter";

interface JourneysStatsCardProps {
  terms: Array<string>;
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
          : "1px solid var(--mantine-color-default-border)",
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

export function JourneysStatsCard({ terms }: JourneysStatsCardProps) {
  const { data, isLoading } = useQuery(browseJourneysStatsQueryOptions(terms));

  return (
    <Paper
      bdrs={0}
      style={{
        borderBlock: "1px solid var(--mantine-color-default-border)",
      }}
    >
      <SimpleGrid cols={4} spacing={0}>
        <StatItem value={data?.journeys} label="Journeys" isLoading={isLoading} />
        <StatItem value={data?.owners} label="Owners" isLoading={isLoading} />
        <StatItem value={data?.posts} label="Posts" isLoading={isLoading} />
        <StatItem
          value={data?.categories}
          label="Categories"
          isLoading={isLoading}
          lastStat
        />
      </SimpleGrid>
    </Paper>
  );
}
