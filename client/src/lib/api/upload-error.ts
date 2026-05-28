import { ApiError } from "./api-error";
import { notify } from "@/integrations/mantine";

export const RATE_LIMIT_MESSAGE =
  "You're going a bit too fast — please wait a moment and try again.";

// Image uploads bypass the BFF and call the .NET API directly (see
// project_bff_upload_exception). That means upload errors never reach the
// TanStack Query global error handler, so the 429 → yellow-warning behavior
// has to be re-applied at each upload call site via this helper.
export function handleUploadError(error: unknown, fallbackMessage: string) {
  if (error instanceof ApiError && error.status === 429) {
    notify.warning({ message: RATE_LIMIT_MESSAGE });
    return;
  }
  notify.error({ message: fallbackMessage });
}
