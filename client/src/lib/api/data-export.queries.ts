import { useMutation } from "@tanstack/react-query";
import { requestDataExportFn } from "./data-export";

/**
 * Fires the "Export my data" request. Errors are surfaced inline at the call site
 * (rate-limit vs. generic), so this opts out of the global mutation error toast via
 * `meta.suppressGlobalError`. A fresh `mutate()` reattempts, covering retry-after-error.
 */
export function useRequestDataExport() {
  return useMutation({
    mutationFn: () => requestDataExportFn(),
    meta: { suppressGlobalError: true },
  });
}
