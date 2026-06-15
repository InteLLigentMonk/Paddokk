import { createServerFn } from "@tanstack/react-start";
import { dataExportRequest } from "@/generated/api/data-export/data-export";

/**
 * Queues a GDPR data export for the authenticated user. The actor is resolved
 * server-side from the session, so a caller can only ever export their own data.
 * Returns the export request — the existing in-flight one if a build is already
 * under way, otherwise a freshly queued request.
 */
export const requestDataExportFn = createServerFn({ method: "POST" }).handler(
  async () => await dataExportRequest(),
);
