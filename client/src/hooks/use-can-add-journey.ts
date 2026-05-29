import { useQuery } from "@tanstack/react-query";
import { userJourneysCanCreateJourney } from "@/generated/api/user-journeys/user-journeys";
import { journeyKeys } from "@/lib/api/journeys.keys";

export function useCanAddJourney() {
  const { data, isLoading } = useQuery({
    queryKey: journeyKeys.journeyLimits,
    queryFn: () => userJourneysCanCreateJourney(),
  });

  const limits = data;
  const isUnlimited = limits?.maxJourneys === "Unlimited";

  return {
    canAdd: limits?.canCreate ?? false,
    currentCount: limits ? Number(limits.currentCount) : undefined,
    maxJourneys: isUnlimited ? null : limits?.maxJourneys,
    isLoading,
  };
}
