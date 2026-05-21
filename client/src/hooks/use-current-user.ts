import { useQuery } from "@tanstack/react-query";
import { useSession } from "@/lib/auth-client";
import { currentUserQueryOptions } from "@/lib/api/users.queries";

/**
 * Resolves the current user's full profile (including username) from the API.
 * Shares the cache key with route loaders that use currentUserQueryOptions,
 * so a hover-preload and a component-mount don't double-fetch. Logout calls
 * queryClient.clear(), so cross-user cache contamination is handled there.
 */
export function useCurrentUser() {
  const { data: session } = useSession();
  return useQuery({
    ...currentUserQueryOptions(),
    enabled: !!session?.user.id,
  });
}
