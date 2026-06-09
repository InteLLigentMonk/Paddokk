import {
  CONSENT_COOKIE_NAME,
  
  serializeConsentRecord
} from "./consent-record";
import type {ConsentRecord} from "./consent-record";

/** One year — long-lived so a recorded decision survives across visits. */
const CONSENT_MAX_AGE_SECONDS = 60 * 60 * 24 * 365;

/**
 * Persist a decision in a first-party, JS-readable cookie. Not HttpOnly by
 * design: the client must read and update it without a server round trip.
 */
export function writeConsentCookie(record: ConsentRecord): void {
  const value = encodeURIComponent(serializeConsentRecord(record));
  document.cookie = `${CONSENT_COOKIE_NAME}=${value}; path=/; max-age=${CONSENT_MAX_AGE_SECONDS}; SameSite=Lax`;
}

/** Clear the stored decision so the banner reappears on next navigation. */
export function clearConsentCookie(): void {
  document.cookie = `${CONSENT_COOKIE_NAME}=; path=/; max-age=0; SameSite=Lax`;
}
