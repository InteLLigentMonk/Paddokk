import { queryOptions } from "@tanstack/react-query";
import { fetchEnabledSocialProviders } from "@/lib/auth/social-providers";
import { socialProvidersKeys } from "@/lib/api/social-providers.keys";

/**
 * Which social OAuth providers are configured on the server. The answer is fixed
 * for the lifetime of a deployment (driven by env), so it never goes stale
 * within a session.
 */
export const enabledSocialProvidersQueryOptions = () =>
  queryOptions({
    queryKey: socialProvidersKeys.enabled,
    queryFn: () => fetchEnabledSocialProviders(),
    staleTime: Infinity,
  });
