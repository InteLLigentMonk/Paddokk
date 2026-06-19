/**
 * Optional error forwarding to Sentry. Sentry is not yet wired up (see #190);
 * until it is, `globalThis.Sentry` is absent and `reportError` is a no-op.
 * Once Sentry initialises and exposes itself globally, caught errors are
 * forwarded with the current route and authenticated actor in scope. The guard
 * means callers behave identically whether or not Sentry is present.
 */

interface SentryLike {
  captureException: (
    error: unknown,
    context?: {
      tags?: Record<string, string>;
      user?: { id: string };
    },
  ) => void;
}

function getSentry(): SentryLike | null {
  const candidate = (globalThis as { Sentry?: SentryLike }).Sentry;
  return candidate ?? null;
}

export interface ErrorReportContext {
  /** The route path where the error was caught (e.g. "/feed"). */
  route: string;
  /** The authenticated user's id, or null for anonymous visitors. */
  actorId: string | null;
}

export function reportError(error: unknown, context: ErrorReportContext): void {
  const sentry = getSentry();
  if (!sentry) return;

  sentry.captureException(error, {
    tags: { route: context.route },
    user: context.actorId ? { id: context.actorId } : undefined,
  });
}
