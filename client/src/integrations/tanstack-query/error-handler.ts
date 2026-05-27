import { notify } from "../mantine";
import { ApiError } from "@/lib/api/api-error";

const RATE_LIMIT_MESSAGE =
  "You're going a bit too fast — please wait a moment and try again.";

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
  return (error: Error) => {
    if (error.name === "AbortError") {
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
