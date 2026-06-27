import { Alert, Button, Stack } from "@mantine/core";
import { CircleCheck, Download } from "lucide-react";
import { useRequestDataExport } from "@/lib/api/data-export.queries";
import { SEVERITY_COLOR, resolveApiError } from "@/lib/api/error-resolver";

const SUCCESS_MESSAGE =
  "Export requested — you'll receive an email when it's ready.";
const FALLBACK_MESSAGE = "Could not request your export. Please try again.";

/**
 * "Export my data" action. Renders the export-request state inline — confirmation
 * on success, a soft warning when rate-limited (EXPORT_COOLDOWN / 429), a retryable
 * error otherwise. Copy and severity come from the shared resolver, keyed on the
 * error code. No page reload; the button itself is the retry affordance.
 */
export function ExportDataAction() {
  const mutation = useRequestDataExport();

  if (mutation.isSuccess) {
    return (
      <Stack gap="xs">
        <Button
          variant="light"
          color="green"
          leftSection={<CircleCheck size={16} />}
          disabled
        >
          Export requested
        </Button>
        <Alert color="green" variant="light">
          {SUCCESS_MESSAGE}
        </Alert>
      </Stack>
    );
  }

  const resolvedError = mutation.isError
    ? resolveApiError(mutation.error, { fallbackMessage: FALLBACK_MESSAGE })
    : null;

  return (
    <Stack gap="xs">
      <Button
        variant="light"
        leftSection={<Download size={16} />}
        loading={mutation.isPending}
        onClick={() => mutation.mutate()}
      >
        Export my data
      </Button>
      {resolvedError && (
        <Alert color={SEVERITY_COLOR[resolvedError.severity]} variant="light">
          {resolvedError.message}
        </Alert>
      )}
    </Stack>
  );
}
