/**
 * Contact address surfaced to users when they need to reach the team
 * (e.g. the "Report a problem" link on the error page). Kept as a single
 * constant so the address lives in one place.
 */
export const SUPPORT_EMAIL = "support@paddokk.com";

/**
 * Builds a "Report a problem" mailto link with a pre-filled subject and body.
 * Used by both the full-page error boundary and the error-code resolver's
 * fallback notification so the support address and message shape live in one place.
 */
export function buildSupportHref(
  subject: string,
  technicalDetail?: string,
): string {
  const encodedSubject = encodeURIComponent(subject);
  const body = encodeURIComponent(
    [
      "I ran into a problem on Paddokk.",
      "",
      "What I was doing:",
      "",
      technicalDetail ? `Technical detail: ${technicalDetail}` : "",
    ].join("\n"),
  );
  return `mailto:${SUPPORT_EMAIL}?subject=${encodedSubject}&body=${body}`;
}
