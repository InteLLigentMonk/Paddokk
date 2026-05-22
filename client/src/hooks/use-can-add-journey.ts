import { useQuery } from "@tanstack/react-query";
import { userJourneysCanCreateJourney } from "@/generated/api/user-journeys/user-journeys";

export function useCanAddJourney() {
  const { data, isLoading } = useQuery({
    queryKey: ["journey-limits"],
    queryFn: () => userJourneysCanCreateJourney(),
  });

  const limits = data?.status === 200 ? data.data : undefined;
  const isUnlimited = limits?.maxJourneys === "Unlimited";

  return {
    canAdd: limits?.canCreate ?? false,
    currentCount: limits ? Number(limits.currentCount) : undefined,
    maxJourneys: isUnlimited ? null : limits?.maxJourneys,
    isLoading,
  };
}
