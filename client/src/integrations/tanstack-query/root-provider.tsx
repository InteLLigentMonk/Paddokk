import { QueryCache, MutationCache, QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createQueryErrorHandler, createMutationErrorHandler } from './error-handler'

export function getContext() {
  const queryClient = new QueryClient({
    queryCache: new QueryCache({
      onError: createQueryErrorHandler(),
    }),
    mutationCache: new MutationCache({
      onError: createMutationErrorHandler(),
    }),
  })
  return {
    queryClient,
  }
}

export function Provider({
  children,
  queryClient,
}: {
  children: React.ReactNode
  queryClient: QueryClient
}) {
  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  )
}
