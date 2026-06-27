import { notifyApiError } from "./notify-api-error";

export { RATE_LIMIT_MESSAGE } from "./error-resolver";

// Image uploads bypass the BFF and call the .NET API directly (see
// project_bff_upload_exception). That means upload errors never reach the
// TanStack Query global error handler, so they are routed through the same
// shared resolver here. Upload validator codes (UPLOAD_TOO_LARGE, ...) get
// specific copy; anything unmapped falls back to the caller's per-surface message.
export function handleUploadError(error: unknown, fallbackMessage: string) {
  notifyApiError(error, { fallbackMessage });
}
