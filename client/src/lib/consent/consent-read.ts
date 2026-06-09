import { createIsomorphicFn } from "@tanstack/react-start";
import {
  CONSENT_COOKIE_NAME,
  
  parseConsentRecord
} from "./consent-record";
import type {ConsentRecord} from "./consent-record";

/**
 * NOTE: This module imports `@tanstack/react-start`, which must not be pulled
 * into any React component/hook module graph — doing so loads a second copy of
 * React under Vitest and breaks hooks. Keep this isolated from consent-context
 * and the consent components; only the route/SSR layer imports it.
 */

/** Read one cookie value out of a raw `Cookie:` header string. */
function readFromCookieHeader(
  header: string | null | undefined,
  name: string,
): string | null {
  if (!header) return null;
  for (const part of header.split(";")) {
    const [rawName, ...rawValue] = part.trim().split("=");
    if (rawName === name) {
      return decodeURIComponent(rawValue.join("="));
    }
  }
  return null;
}

/**
 * Read the consent record isomorphically so SSR and the first client render
 * agree, avoiding a banner flash / hydration mismatch. On the server the value
 * comes from the request's Cookie header; on the client from document.cookie.
 */
export const readConsentRecord = createIsomorphicFn()
  .server(async (): Promise<ConsentRecord | null> => {
    try {
      const { getRequestHeader } = await import("@tanstack/react-start/server");
      return parseConsentRecord(
        readFromCookieHeader(getRequestHeader("cookie"), CONSENT_COOKIE_NAME),
      );
    } catch {
      return null;
    }
  })
  .client((): ConsentRecord | null =>
    parseConsentRecord(
      readFromCookieHeader(document.cookie, CONSENT_COOKIE_NAME),
    ),
  );
