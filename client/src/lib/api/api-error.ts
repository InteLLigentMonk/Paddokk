/** A single field-level validation failure from the API error envelope. */
export interface ApiFieldError {
  field: string;
  code: string;
  message: string;
}

/**
 * Thrown by {@link apiFetcher} for every non-OK response. Carries the backend's
 * unified error envelope: a machine-readable {@link code} (branched on by the
 * error-code resolver), the diagnostic {@link message} (never rendered to users —
 * see ADR-0007), the HTTP {@link status}, optional per-field validation
 * {@link errors}, and the per-request {@link traceId} for support correlation.
 */
export class ApiError extends Error {
  status: number;
  code: string;
  errors?: ReadonlyArray<ApiFieldError>;
  traceId?: string;

  constructor(
    status: number,
    message: string,
    code: string,
    errors?: ReadonlyArray<ApiFieldError>,
    traceId?: string,
  ) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.code = code;
    this.errors = errors;
    this.traceId = traceId;
  }
}

/**
 * Structural guard for {@link ApiError}. Use this instead of `instanceof` for any
 * check on the client: errors thrown by `apiFetcher` inside a BFF server function
 * are serialized across the network by seroval, which reconstructs them as a plain
 * `Error` (losing the `ApiError` prototype) while preserving `name`, `code`,
 * `status`, and `errors`. `instanceof ApiError` is therefore always false on the
 * client. This guard matches both the real class and its deserialized shape.
 */
export function isApiError(error: unknown): error is ApiError {
  return (
    error instanceof ApiError ||
    (error instanceof Error &&
      error.name === "ApiError" &&
      typeof (error as Partial<ApiError>).code === "string" &&
      typeof (error as Partial<ApiError>).status === "number")
  );
}
