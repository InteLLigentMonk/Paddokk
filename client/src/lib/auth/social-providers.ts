import { createServerFn } from "@tanstack/react-start";
import { getEnabledSocialProviders } from "./social-config";

/**
 * Exposes which social OAuth providers are configured on the server so the UI
 * can enable only the buttons that will actually work. Reads env only — it
 * deliberately does not import the BetterAuth instance, keeping the import graph
 * light and free of the db/email side effects in `auth.server.ts`.
 *
 * Not named `*.server.ts`: createServerFn already strips the handler from the
 * client bundle and bridges it over RPC, so this module is safe to import from
 * client code. The `.server.ts` suffix would instead trip import-protection.
 */
export const fetchEnabledSocialProviders = createServerFn({
  method: "GET",
}).handler(() => getEnabledSocialProviders());
