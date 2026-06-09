import { describe, expect, it } from "vitest";
import {
  CONSENT_POLICY_VERSION,
  allowsNonEssential,
  buildConsentRecord,
  parseConsentRecord,
  serializeConsentRecord,
} from "./consent-record";

describe("parseConsentRecord", () => {
  it("returns null for absent values", () => {
    expect(parseConsentRecord(null)).toBeNull();
    expect(parseConsentRecord(undefined)).toBeNull();
    expect(parseConsentRecord("")).toBeNull();
  });

  it("returns null for malformed JSON", () => {
    expect(parseConsentRecord("not-json")).toBeNull();
  });

  it("returns null when the decision is invalid", () => {
    const raw = JSON.stringify({
      decision: "maybe",
      timestamp: new Date().toISOString(),
      policyVersion: CONSENT_POLICY_VERSION,
    });
    expect(parseConsentRecord(raw)).toBeNull();
  });

  it("returns null when recorded against a superseded policy version", () => {
    const raw = JSON.stringify({
      decision: "all",
      timestamp: new Date().toISOString(),
      policyVersion: "1999-01",
    });
    expect(parseConsentRecord(raw)).toBeNull();
  });

  it("parses a valid record", () => {
    const record = buildConsentRecord("all");
    const parsed = parseConsentRecord(serializeConsentRecord(record));
    expect(parsed).toEqual(record);
  });
});

describe("buildConsentRecord", () => {
  it("stamps the current policy version and an ISO timestamp", () => {
    const record = buildConsentRecord("essential");
    expect(record.decision).toBe("essential");
    expect(record.policyVersion).toBe(CONSENT_POLICY_VERSION);
    expect(() => new Date(record.timestamp).toISOString()).not.toThrow();
    expect(new Date(record.timestamp).toISOString()).toBe(record.timestamp);
  });
});

describe("allowsNonEssential", () => {
  it("is false when there is no decision", () => {
    expect(allowsNonEssential(null)).toBe(false);
  });

  it("is false for an essentials-only decision", () => {
    expect(allowsNonEssential(buildConsentRecord("essential"))).toBe(false);
  });

  it("is true only for accept-all", () => {
    expect(allowsNonEssential(buildConsentRecord("all"))).toBe(true);
  });
});
