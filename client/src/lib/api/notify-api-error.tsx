import { Anchor } from "@mantine/core";
import { resolveApiError } from "./error-resolver";
import type { ResolveOptions, ResolvedError } from "./error-resolver";
import type { ReactNode } from "react";
import { notify } from "@/integrations/mantine";

const SHOW_BY_SEVERITY = {
  error: notify.error,
  warning: notify.warning,
  info: notify.info,
} as const;

function buildMessage(resolved: ResolvedError): ReactNode {
  if (!resolved.cta) return resolved.message;
  return (
    <span>
      {resolved.message}{" "}
      <Anchor href={resolved.cta.href}>{resolved.cta.label}</Anchor>
    </span>
  );
}

/**
 * Resolves any thrown value to user-facing copy and shows the matching Mantine
 * notification. The one entry point shared by the global query/mutation handlers,
 * the direct-upload error shim, and any mutation surfacing its own error inline,
 * so a code maps to the same message everywhere. Silent for aborted requests.
 */
export function notifyApiError(error: unknown, options?: ResolveOptions): void {
  const resolved = resolveApiError(error, options);
  if (!resolved) return;
  SHOW_BY_SEVERITY[resolved.severity]({ message: buildMessage(resolved) });
}
