import { useQuery } from "@tanstack/react-query";
import { currentUserQueryOptions } from "@/lib/api/users.queries";

/**
 * Resolves the current user's full profile (including username) from the API.
 * Shares the cache key with route loaders that use currentUserQueryOptions,
 * so a hover-preload and a component-mount don't double-fetch. Logout calls
 * queryClient.clear(), so cross-user cache contamination is handled there.
 * Only use within authenticated routes (/_app) where currentUser is prefetched.
 */
export function useCurrentUser() {
  return useQuery(currentUserQueryOptions());
}
