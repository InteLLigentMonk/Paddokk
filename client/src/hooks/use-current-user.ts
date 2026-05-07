import { useQuery } from "@tanstack/react-query";
import { useSession } from "@/lib/auth-client";
import { getCurrentUserFn } from "@/lib/api/users.server";

/**
 * Resolves the current user's full profile (including username) from the API.
 * The cache key is scoped by BetterAuth session user id so the query refetches
 * automatically when a different user logs in — otherwise the previous user's
 * profile would still serve from cache.
 */
export function useCurrentUser() {
  const { data: session } = useSession();
  const userId = session?.user?.id ?? null;

  return useQuery({
    queryKey: ["current-user", userId],
    queryFn: () => getCurrentUserFn(),
    enabled: !!userId,
    staleTime: 5 * 60 * 1000,
  });
}
