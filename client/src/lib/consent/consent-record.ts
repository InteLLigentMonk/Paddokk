/**
 * Pure consent-record logic: types, the policy version constant, and the
 * parse/serialize helpers used on both server and client. No I/O here so this
 * stays trivially unit-testable.
 */

import { LEGAL_VERSION } from "@/lib/legal/legal-version";

export const CONSENT_COOKIE_NAME = "paddokk_consent";

/**
 * The consent policy tracks the legal-document version (single source of truth
 * in `@/lib/legal/legal-version`). Bumping it re-prompts every visitor on their
 * next visit, because a stored record whose policyVersion no longer matches is
 * treated as "no decision".
 */
export const CONSENT_POLICY_VERSION = LEGAL_VERSION;

export type ConsentDecision = "all" | "essential";

export interface ConsentRecord {
  decision: ConsentDecision;
  /** ISO-8601 timestamp of when the decision was made. */
  timestamp: string;
  policyVersion: string;
}

/**
 * Parse a raw cookie value into a ConsentRecord, or null when it is absent,
 * malformed, or recorded against a superseded policy version (which re-prompts).
 */
export function parseConsentRecord(
  raw: string | null | undefined,
): ConsentRecord | null {
  if (!raw) return null;

  let parsed: unknown;
  try {
    parsed = JSON.parse(raw);
  } catch {
    return null;
  }

  if (typeof parsed !== "object" || parsed === null) return null;
  const record = parsed as Record<string, unknown>;

  if (record.decision !== "all" && record.decision !== "essential") return null;
  if (typeof record.timestamp !== "string") return null;
  if (typeof record.policyVersion !== "string") return null;

  // A decision made against an older policy is no longer valid consent.
  if (record.policyVersion !== CONSENT_POLICY_VERSION) return null;

  return {
    decision: record.decision,
    timestamp: record.timestamp,
    policyVersion: record.policyVersion,
  };
}

/** Build a fresh record for the given decision, stamped now and to the current policy. */
export function buildConsentRecord(decision: ConsentDecision): ConsentRecord {
  return {
    decision,
    timestamp: new Date().toISOString(),
    policyVersion: CONSENT_POLICY_VERSION,
  };
}

export function serializeConsentRecord(record: ConsentRecord): string {
  return JSON.stringify(record);
}

/** Whether non-essential cookies/functions are permitted for this record. */
export function allowsNonEssential(record: ConsentRecord | null): boolean {
  return record?.decision === "all";
}
