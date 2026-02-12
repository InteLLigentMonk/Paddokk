import type { UseQueryOptions } from '@tanstack/react-query'

/**
 * Mobile-optimized query defaults
 * Reduces network usage and improves perceived performance
 */
export const mobileQueryDefaults: Partial<UseQueryOptions> = {
  staleTime: 5 * 60 * 1000, // 5 minutes - reduce refetches
  gcTime: 10 * 60 * 1000, // 10 minutes - keep cache longer
  refetchOnWindowFocus: false, // Avoid refetch on tab switch
  refetchOnReconnect: true, // Refetch when connection restored
  retry: 2, // Reduce retries to save mobile data
}
