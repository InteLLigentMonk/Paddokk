import { NETWORK_ERROR_MESSAGE, isNetworkError } from "./error-resolver";
import { notifyApiError, notifyRetryableError } from "./notify-api-error";

// Image uploads bypass the BFF and call the .NET API directly (see
// project_bff_upload_exception). That means upload errors never reach the
// TanStack Query global error handler, so they are routed through the same
// shared resolver here:
//   - Validator codes (UPLOAD_TOO_LARGE, UPLOAD_UNSUPPORTED_FORMAT, ...) get
//     specific, actionable copy.
//   - Connection failures get a Retry CTA when the caller passes `onRetry` (only
//     surfaces where re-running the upload alone is safe should do so).
//   - Anything else falls through to the generic "Something went wrong" toast
//     with a "Report a problem" link.
export function handleUploadError(error: unknown, onRetry?: () => void) {
  if (onRetry && isNetworkError(error)) {
    notifyRetryableError(NETWORK_ERROR_MESSAGE, onRetry);
    return;
  }
  notifyApiError(error);
}
