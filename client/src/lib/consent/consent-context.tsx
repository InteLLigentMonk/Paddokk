import {
  
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState
} from "react";
import { clearConsentCookie, writeConsentCookie } from "./consent-cookie";
import {
  
  allowsNonEssential,
  buildConsentRecord
} from "./consent-record";
import type {ReactNode} from "react";
import type {ConsentRecord} from "./consent-record";

/**
 * Public consent state for components and SDK initializers. `essential` is
 * always granted; anything non-essential (analytics, Sentry replay/perf) must
 * gate on `nonEssential`.
 */
export interface ConsentState {
  essential: true;
  nonEssential: boolean;
}

interface ConsentContextValue extends ConsentState {
  /** True once the visitor has made (and we have stored) any decision. */
  hasDecision: boolean;
  acceptAll: () => void;
  rejectNonEssential: () => void;
  /** Clear the stored decision so the banner reappears. */
  reset: () => void;
}

const ConsentContext = createContext<ConsentContextValue | null>(null);

export function ConsentProvider({
  initialRecord,
  children,
}: {
  /** Record read during SSR so the first render matches the client. */
  initialRecord: ConsentRecord | null;
  children: ReactNode;
}) {
  const [record, setRecord] = useState<ConsentRecord | null>(initialRecord);

  const acceptAll = useCallback(() => {
    const next = buildConsentRecord("all");
    writeConsentCookie(next);
    setRecord(next);
  }, []);

  const rejectNonEssential = useCallback(() => {
    const next = buildConsentRecord("essential");
    writeConsentCookie(next);
    setRecord(next);
  }, []);

  const reset = useCallback(() => {
    clearConsentCookie();
    setRecord(null);
  }, []);

  const value = useMemo<ConsentContextValue>(
    () => ({
      essential: true,
      nonEssential: allowsNonEssential(record),
      hasDecision: record !== null,
      acceptAll,
      rejectNonEssential,
      reset,
    }),
    [record, acceptAll, rejectNonEssential, reset],
  );

  return (
    <ConsentContext.Provider value={value}>{children}</ConsentContext.Provider>
  );
}

function useConsentContext(): ConsentContextValue {
  const value = useContext(ConsentContext);
  if (!value) {
    throw new Error("useConsent must be used within a ConsentProvider");
  }
  return value;
}

/**
 * Read consent for gating non-essential features. Returns the documented
 * `{ essential, nonEssential }` shape; defaults to essentials-only until a
 * decision is recorded.
 */
export function useConsent(): ConsentState {
  const { essential, nonEssential } = useConsentContext();
  return { essential, nonEssential };
}

/** Decision state and actions for the banner and "Manage cookies" controls. */
export function useConsentControls() {
  const { hasDecision, acceptAll, rejectNonEssential, reset } =
    useConsentContext();
  return { hasDecision, acceptAll, rejectNonEssential, reset };
}
