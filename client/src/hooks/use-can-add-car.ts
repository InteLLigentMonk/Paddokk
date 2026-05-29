import { useQuery } from "@tanstack/react-query";
import { limitsGetCarLimits } from "@/generated/api/limits/limits";
import { carKeys } from "@/lib/api/cars.keys";

export function useCanAddCar() {
  const { data, isLoading } = useQuery({
    queryKey: carKeys.carLimits,
    queryFn: () => limitsGetCarLimits(),
  });

  const limits = data;

  return {
    canAdd: limits?.canAdd ?? false,
    isLoading,
  };
}
