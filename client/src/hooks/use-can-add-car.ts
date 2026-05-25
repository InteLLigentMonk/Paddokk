import { useQuery } from "@tanstack/react-query";
import { limitsGetCarLimits } from "@/generated/api/limits/limits";

export function useCanAddCar() {
  const { data, isLoading } = useQuery({
    queryKey: ["car-limits"],
    queryFn: () => limitsGetCarLimits(),
  });

  const limits = data;

  return {
    canAdd: limits?.canAdd ?? false,
    isLoading,
  };
}
