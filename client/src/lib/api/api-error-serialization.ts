import { createSerializationAdapter } from "@tanstack/react-router";
import { ApiError } from "./api-error";
import type { ApiFieldError } from "./api-error";

/**
 * Preserves {@link ApiError} across the BFF server-function boundary.
 *
 * TanStack Router registers a built-in `ShallowErrorPlugin` that matches any
 * `value instanceof Error` and serializes ONLY its `message`, discarding `code`,
 * `status`, `errors`, and the prototype. Without this adapter, every API error
 * reaches the client as a bare `new Error(message)`, so the error-code resolver
 * never sees a `code` and falls back to the generic "Something went wrong" copy.
 *
 * Registered via `serializationAdapters` in `src/start.ts`, which seroval places
 * BEFORE the default plugins. seroval uses the first plugin whose `test` matches,
 * and `instanceof ApiError` is more specific than `ShallowErrorPlugin`'s
 * `instanceof Error`, so this adapter wins and the full envelope survives.
 */
interface SerializedApiError {
  status: number;
  message: string;
  code: string;
  errors?: ReadonlyArray<ApiFieldError>;
  traceId?: string;
}

export const apiErrorSerializationAdapter = createSerializationAdapter({
  key: "ApiError",
  test: (value): value is ApiError => value instanceof ApiError,
  toSerializable: (error: ApiError): SerializedApiError => ({
    status: error.status,
    message: error.message,
    code: error.code,
    errors: error.errors,
    traceId: error.traceId,
  }),
  fromSerializable: (data: SerializedApiError): ApiError =>
    new ApiError(
      data.status,
      data.message,
      data.code,
      data.errors,
      data.traceId,
    ),
});
