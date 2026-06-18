import { afterEach, beforeEach, describe, expect, it } from "vitest";
import {
  getEnabledSocialProviders,
  getSocialProviderCredentials,
} from "./social-config";

const OAUTH_ENV_KEYS = [
  "GOOGLE_CLIENT_ID",
  "GOOGLE_CLIENT_SECRET",
  "FACEBOOK_CLIENT_ID",
  "FACEBOOK_CLIENT_SECRET",
] as const;

describe("social-config", () => {
  const original: Record<string, string | undefined> = {};

  beforeEach(() => {
    for (const key of OAUTH_ENV_KEYS) {
      original[key] = process.env[key];
      delete process.env[key];
    }
  });

  afterEach(() => {
    for (const key of OAUTH_ENV_KEYS) {
      if (original[key] === undefined) delete process.env[key];
      else process.env[key] = original[key];
    }
  });

  describe("getSocialProviderCredentials", () => {
    it("omits a provider when its env vars are absent", () => {
      const creds = getSocialProviderCredentials();
      expect(creds.google).toBeUndefined();
      expect(creds.facebook).toBeUndefined();
    });

    it("omits a provider when only one of id/secret is set", () => {
      process.env.GOOGLE_CLIENT_ID = "google-id";
      // GOOGLE_CLIENT_SECRET intentionally missing
      expect(getSocialProviderCredentials().google).toBeUndefined();
    });

    it("returns credentials when both id and secret are set", () => {
      process.env.GOOGLE_CLIENT_ID = "google-id";
      process.env.GOOGLE_CLIENT_SECRET = "google-secret";
      expect(getSocialProviderCredentials().google).toEqual({
        clientId: "google-id",
        clientSecret: "google-secret",
      });
    });
  });

  describe("getEnabledSocialProviders", () => {
    it("reports both disabled when no credentials are set", () => {
      expect(getEnabledSocialProviders()).toEqual({
        google: false,
        facebook: false,
      });
    });

    it("reports a provider enabled only when fully configured", () => {
      process.env.FACEBOOK_CLIENT_ID = "fb-id";
      process.env.FACEBOOK_CLIENT_SECRET = "fb-secret";
      expect(getEnabledSocialProviders()).toEqual({
        google: false,
        facebook: true,
      });
    });
  });
});
