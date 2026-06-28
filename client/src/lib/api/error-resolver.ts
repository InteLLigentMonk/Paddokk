import { isApiError } from "./api-error";
import { buildSupportHref } from "@/lib/support";

/**
 * Machine-readable error codes from the backend's unified error envelope (see
 * `ErrorCodes.cs`). The frontend branches on these to render specific, actionable
 * copy. `EmptyResponse` is client-only (synthesised by `requirePage`).
 */
export const ApiErrorCode = {
  NotFound: "NOT_FOUND",
  Conflict: "CONFLICT",
  Forbidden: "FORBIDDEN",
  ValidationFailed: "VALIDATION_FAILED",
  Internal: "INTERNAL",
  RateLimited: "RATE_LIMITED",
  RequestCancelled: "REQUEST_CANCELLED",
  SubscribeToOwnSubject: "SUBSCRIBE_TO_OWN_SUBJECT",
  LikeOwnSubject: "LIKE_OWN_SUBJECT",
  UsernameTaken: "USERNAME_TAKEN",
  UsernameReserved: "USERNAME_RESERVED",
  UsernameChangeTooSoon: "USERNAME_CHANGE_TOO_SOON",
  ExportCooldown: "EXPORT_COOLDOWN",
  UploadRequired: "UPLOAD_REQUIRED",
  UploadTooLarge: "UPLOAD_TOO_LARGE",
  UploadUnsupportedFormat: "UPLOAD_UNSUPPORTED_FORMAT",
  UploadContentMismatch: "UPLOAD_CONTENT_MISMATCH",
  UploadDimensionsTooSmall: "UPLOAD_DIMENSIONS_TOO_SMALL",
  UploadDimensionsTooLarge: "UPLOAD_DIMENSIONS_TOO_LARGE",
  UploadInvalidImage: "UPLOAD_INVALID_IMAGE",
  EmptyResponse: "EMPTY_RESPONSE",
} as const;

export type ErrorSeverity = "error" | "warning" | "info";

/**
 * Maps a resolved severity to a Mantine color. Shared by the toast notifications
 * and any inline `<Alert>` surfacing a resolved error, so a code reads with the
 * same colour wherever it appears.
 */
export const SEVERITY_COLOR: Record<ErrorSeverity, string> = {
  error: "red",
  warning: "yellow",
  info: "blue",
};

/** Optional call-to-action rendered alongside a notification (e.g. "Report a problem"). */
export interface ResolvedErrorCta {
  label: string;
  href: string;
}

/** A code mapped to user-facing copy: what to say, how loudly, and an optional CTA. */
export interface ResolvedError {
  message: string;
  severity: ErrorSeverity;
  cta?: ResolvedErrorCta;
}

export const RATE_LIMIT_MESSAGE =
  "You're going a bit too fast — please wait a moment and try again.";

export const FALLBACK_MESSAGE = "Something went wrong. Try again.";

export const NETWORK_ERROR_MESSAGE =
  "We couldn't reach the server. Check your connection and try again.";

export interface ResolveOptions {
  /**
   * Overrides the generic fallback copy for unmapped codes (e.g. a per-surface
   * upload message). When supplied, the generic "Report a problem" CTA is dropped.
   */
  fallbackMessage?: string;
}

/** Curated copy for the codes we surface specifically. Unmapped codes hit the fallback. */
const CODE_MESSAGES: Partial<Record<string, ResolvedError>> = {
  [ApiErrorCode.SubscribeToOwnSubject]: {
    message: "You can't follow your own journey or car.",
    severity: "error",
  },
  [ApiErrorCode.LikeOwnSubject]: {
    message: "You can't like your own journey or car.",
    severity: "error",
  },
  [ApiErrorCode.NotFound]: {
    message: "We couldn't find what you were looking for.",
    severity: "error",
  },
  [ApiErrorCode.Forbidden]: {
    message: "You don't have permission to do that.",
    severity: "error",
  },
  [ApiErrorCode.UsernameTaken]: {
    message: "That username is already taken. Please choose another.",
    severity: "error",
  },
  [ApiErrorCode.UsernameReserved]: {
    message: "That username isn't available. Please choose another.",
    severity: "error",
  },
  [ApiErrorCode.UsernameChangeTooSoon]: {
    message:
      "You can only change your username once every 30 days. Please try again later.",
    severity: "warning",
  },
  [ApiErrorCode.ExportCooldown]: {
    message:
      "You requested an export recently. Please wait before requesting again.",
    severity: "warning",
  },
  [ApiErrorCode.UploadRequired]: {
    message: "Please choose a file to upload.",
    severity: "error",
  },
  [ApiErrorCode.UploadTooLarge]: {
    message: "Your image is too large. Maximum size is 5 MB.",
    severity: "error",
  },
  [ApiErrorCode.UploadUnsupportedFormat]: {
    message: "Only JPEG, PNG, and WebP images are supported.",
    severity: "error",
  },
  [ApiErrorCode.UploadContentMismatch]: {
    message: "That file doesn't look like a valid image.",
    severity: "error",
  },
  [ApiErrorCode.UploadInvalidImage]: {
    message: "That file doesn't look like a valid image. Please try another.",
    severity: "error",
  },
  [ApiErrorCode.UploadDimensionsTooSmall]: {
    message: "Your image is too small. Please use one at least 100×100 pixels.",
    severity: "error",
  },
  [ApiErrorCode.UploadDimensionsTooLarge]: {
    message:
      "Your image's dimensions are too large. The maximum is 4000×4000 pixels.",
    severity: "error",
  },
};

function isAbort(error: unknown): boolean {
  return error instanceof Error && error.name === "AbortError";
}

/**
 * True when a failure is a connection/transport problem rather than a structured
 * backend response — i.e. `fetch` itself rejected (offline, DNS, connection
 * refused), surfacing as a `TypeError` with no error envelope. Retrying makes
 * sense here, so the upload shim offers a Retry CTA for these. User- or
 * navigation-initiated aborts are excluded: they are intentional, not failures.
 */
export function isNetworkError(error: unknown): boolean {
  return error instanceof Error && !isApiError(error) && !isAbort(error);
}

/**
 * True when an error means "the resource does not exist". Route loaders use this to
 * decide between the 404 `notFoundComponent` (this returns true) and the 500
 * `errorComponent` (everything else rethrows). See ADR-0007.
 */
export function isNotFoundError(error: unknown): boolean {
  const apiError = isApiError(error) ? error : undefined;
  return apiError?.status === 404 || apiError?.code === ApiErrorCode.NotFound;
}

function fallback(message?: string): ResolvedError {
  if (message) {
    return { message, severity: "error" };
  }
  return {
    message: FALLBACK_MESSAGE,
    severity: "error",
    cta: {
      label: "Report a problem",
      href: buildSupportHref("Problem report"),
    },
  };
}

/**
 * Maps any thrown value to user-facing notification copy. The single source of
 * truth for how an API failure is surfaced. Returns `null` when nothing should be
 * shown (aborted or server-cancelled requests).
 */
export function resolveApiError(
  error: unknown,
  options?: ResolveOptions,
): ResolvedError | null {
  // Aborted requests are user- or navigation-initiated; never notify.
  if (isAbort(error)) return null;

  const apiError = isApiError(error) ? error : undefined;
  const code = apiError?.code;
  const status = apiError?.status;

  if (code === ApiErrorCode.RequestCancelled) return null;

  // Rate limiting can arrive as a 429 status or an explicit code; soften to a warning.
  if (status === 429 || code === ApiErrorCode.RateLimited) {
    return { message: RATE_LIMIT_MESSAGE, severity: "warning" };
  }

  const mapped = code ? CODE_MESSAGES[code] : undefined;
  if (mapped) return mapped;

  return fallback(options?.fallbackMessage);
}
