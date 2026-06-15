import { Alert, Button, Stack } from "@mantine/core";
import { CircleCheck, Download } from "lucide-react";
import { useRequestDataExport } from "@/lib/api/data-export.queries";
import { ApiError } from "@/lib/api/api-error";

const SUCCESS_MESSAGE =
  "Export requested — you'll receive an email when it's ready.";
const COOLDOWN_MESSAGE =
  "You requested an export recently — please wait before requesting again.";
const THROTTLED_MESSAGE =
  "You're going a bit too fast — please wait a moment and try again.";
const ERROR_MESSAGE = "Could not request your export. Please try again.";

function rateLimitMessage(error: unknown): string | null {
  if (!(error instanceof ApiError)) return null;
  if (error.status === 409) return COOLDOWN_MESSAGE;
  if (error.status === 429) return THROTTLED_MESSAGE;
  return null;
}

/**
 * "Export my data" action. Renders the export-request state inline — confirmation
 * on success, a soft warning when rate-limited, a retryable error otherwise. No
 * page reload; the button itself is the retry affordance.
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

  const rateLimited = mutation.isError
    ? rateLimitMessage(mutation.error)
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
      {rateLimited && (
        <Alert color="yellow" variant="light">
          {rateLimited}
        </Alert>
      )}
      {mutation.isError && !rateLimited && (
        <Alert color="red" variant="light">
          {ERROR_MESSAGE}
        </Alert>
      )}
    </Stack>
  );
}
