import { useQuery } from "@tanstack/react-query";
import { limitsGetCarLimits } from "@/generated/api/limits/limits";

export function useCanAddCar() {
  const { data, isLoading } = useQuery({
    queryKey: ["car-limits"],
    queryFn: () => limitsGetCarLimits(),
  });

  const limits = data?.status === 200 ? data.data : undefined;

  return {
    canAdd: limits?.canAdd ?? false,
    isLoading,
  };
}
