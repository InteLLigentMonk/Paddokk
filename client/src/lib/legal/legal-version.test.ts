import { describe, expect, it } from "vitest";
import {
  LEGAL_VERSION,
  LEGAL_VERSION_LABEL,
  formatLegalVersionLabel,
} from "./legal-version";
import { CONSENT_POLICY_VERSION } from "@/lib/consent";

describe("LEGAL_VERSION", () => {
  it("is a machine-sortable YYYY-MM string", () => {
    expect(LEGAL_VERSION).toMatch(/^\d{4}-\d{2}$/);
  });

  it("is the single source of truth the consent policy version tracks", () => {
    // Bumping LEGAL_VERSION must re-prompt consent, so they must stay equal.
    expect(CONSENT_POLICY_VERSION).toBe(LEGAL_VERSION);
  });
});

describe("formatLegalVersionLabel", () => {
  it("renders a human-readable month and year", () => {
    expect(formatLegalVersionLabel("2026-05")).toBe("May 2026");
    expect(formatLegalVersionLabel("2026-01")).toBe("January 2026");
    expect(formatLegalVersionLabel("2025-12")).toBe("December 2025");
  });

  it("backs the exported label", () => {
    expect(LEGAL_VERSION_LABEL).toBe(formatLegalVersionLabel(LEGAL_VERSION));
  });
});
