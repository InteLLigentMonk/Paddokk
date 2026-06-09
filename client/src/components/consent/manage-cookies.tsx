import { Anchor, Button } from "@mantine/core";
import { useConsentControls } from "@/lib/consent/consent-context";

/**
 * Clears the stored consent decision so the banner reappears on next
 * navigation. Renders as a dimmed footer link or a settings button depending
 * on `variant`, sharing the same reset wiring.
 */
export function ManageCookies({
  variant = "link",
  label = "Manage cookies",
}: {
  variant?: "link" | "button";
  label?: string;
}) {
  const { reset } = useConsentControls();

  if (variant === "button") {
    return (
      <Button variant="default" onClick={reset}>
        {label}
      </Button>
    );
  }

  return (
    <Anchor
      component="button"
      type="button"
      onClick={reset}
      fz="sm"
      c="dimmed"
      underline="never"
      ta="left"
    >
      {label}
    </Anchor>
  );
}
