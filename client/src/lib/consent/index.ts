export {
  CONSENT_COOKIE_NAME,
  CONSENT_POLICY_VERSION,
  type ConsentDecision,
  type ConsentRecord,
} from "./consent-record";
export { readConsentRecord } from "./consent-read";
export {
  ConsentProvider,
  type ConsentState,
  useConsent,
  useConsentControls,
} from "./consent-context";
