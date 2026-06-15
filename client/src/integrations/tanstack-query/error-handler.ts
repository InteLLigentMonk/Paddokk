import { notify } from "../mantine";
import type { Mutation } from "@tanstack/react-query";
import { ApiError } from "@/lib/api/api-error";
import { RATE_LIMIT_MESSAGE } from "@/lib/api/upload-error";

function isRateLimitError(error: Error): boolean {
  return error instanceof ApiError && error.status === 429;
}

export function createQueryErrorHandler() {
  return (error: Error) => {
    if (error.name === "AbortError") {
      return;
    }

    if (isRateLimitError(error)) {
      notify.warning({ message: RATE_LIMIT_MESSAGE });
      return;
    }

    notify.error({
      message: error.message || "An error occurred while fetching data",
    });
  };
}

export function createMutationErrorHandler() {
  return (
    error: Error,
    _variables: unknown,
    _context: unknown,
    mutation: Mutation<unknown, unknown, unknown, unknown>,
  ) => {
    if (error.name === "AbortError") {
      return;
    }

    // Some mutations surface errors inline at the call site and opt out of the toast.
    if (mutation.meta?.suppressGlobalError) {
      return;
    }

    if (isRateLimitError(error)) {
      notify.warning({ message: RATE_LIMIT_MESSAGE });
      return;
    }

    notify.error({
      message: error.message || "An error occurred while saving data",
    });
  };
}
