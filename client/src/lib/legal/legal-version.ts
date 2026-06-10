/**
 * Single source of truth for the legal-document version.
 *
 * The Privacy Policy and Terms of Service stamp their "Last updated" date from
 * here, and the cookie-consent module tracks this same value as its policy
 * version (see `consent-record.ts`). Bumping `LEGAL_VERSION` therefore both
 * restamps the legal pages and re-prompts every visitor for fresh consent on
 * their next visit. See `docs/legal-version-bump.md` for the procedure.
 */

/** Machine-sortable version stamp in `YYYY-MM` form. */
export const LEGAL_VERSION = "2026-05";

const MONTH_NAMES = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
] as const;

/** Turn a `YYYY-MM` version into a human-readable "Month YYYY" label. */
export function formatLegalVersionLabel(version: string): string {
  const [year, month] = version.split("-");
  const monthName = MONTH_NAMES[Number(month) - 1];
  return `${monthName} ${year}`;
}

/** Human-readable label rendered as the "Last updated" stamp on legal pages. */
export const LEGAL_VERSION_LABEL = formatLegalVersionLabel(LEGAL_VERSION);
