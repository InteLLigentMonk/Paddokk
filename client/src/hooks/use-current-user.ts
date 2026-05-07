import { useQuery } from "@tanstack/react-query";
import { getCurrentUserFn } from "@/lib/api/users.server";

/**
 * Resolves the current user's full profile (including username) from the API.
 * Cached via React Query so this is safe to call from multiple components.
 */
export function useCurrentUser() {
  return useQuery({
    queryKey: ["current-user"],
    queryFn: () => getCurrentUserFn(),
    staleTime: 5 * 60 * 1000,
  });
}
