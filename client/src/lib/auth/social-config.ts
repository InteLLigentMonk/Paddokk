/**
 * Resolution of social OAuth provider credentials from env.
 *
 * Intended for server use (the BetterAuth config and the enabled-providers
 * server function). Credentials live in non-`VITE_` env vars so they are never
 * inlined into the client bundle. A provider is only considered configured when
 * BOTH its client id and secret are present — this lets the auth config register
 * only usable providers and lets the UI disable buttons for providers that
 * haven't been provisioned yet.
 */

export type SocialProvider = "google" | "facebook";

export interface ProviderCredentials {
  clientId: string;
  clientSecret: string;
}

export type SocialProviderCredentials = Partial<
  Record<SocialProvider, ProviderCredentials>
>;

export type EnabledSocialProviders = Record<SocialProvider, boolean>;

const ENV_KEYS: Record<SocialProvider, { id: string; secret: string }> = {
  google: { id: "GOOGLE_CLIENT_ID", secret: "GOOGLE_CLIENT_SECRET" },
  facebook: { id: "FACEBOOK_CLIENT_ID", secret: "FACEBOOK_CLIENT_SECRET" },
};

function readCredentials(
  provider: SocialProvider,
): ProviderCredentials | undefined {
  const { id, secret } = ENV_KEYS[provider];
  const clientId = process.env[id];
  const clientSecret = process.env[secret];
  if (!clientId || !clientSecret) return undefined;
  return { clientId, clientSecret };
}

export function getSocialProviderCredentials(): SocialProviderCredentials {
  return {
    google: readCredentials("google"),
    facebook: readCredentials("facebook"),
  };
}

export function getEnabledSocialProviders(): EnabledSocialProviders {
  const credentials = getSocialProviderCredentials();
  return {
    google: credentials.google !== undefined,
    facebook: credentials.facebook !== undefined,
  };
}
