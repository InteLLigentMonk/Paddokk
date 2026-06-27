import type { Mutation, Query } from "@tanstack/react-query";
import { notifyApiError } from "@/lib/api/notify-api-error";

export function createQueryErrorHandler() {
  return (error: Error, query: Query<unknown, unknown>) => {
    // Route-critical queries are fetched by a loader (ensureQueryData/prefetch) with no
    // observers attached; their failures are owned by the route's notFound/error boundary,
    // so toasting here would double-surface the same error (see ADR-0007). Once a component
    // mounts and observes the query, a (re)fetch failure is a widget error and does toast.
    if (query.getObserversCount() === 0) return;
    notifyApiError(error);
  };
}

export function createMutationErrorHandler() {
  return (
    error: Error,
    _variables: unknown,
    _context: unknown,
    mutation: Mutation<unknown, unknown, unknown, unknown>,
  ) => {
    // Some mutations surface errors inline at the call site and opt out of the toast.
    if (mutation.meta?.suppressGlobalError) {
      return;
    }
    notifyApiError(error);
  };
}
