# Bumping the legal/consent version

The Privacy Policy and Terms of Service carry a version stamp, and the
cookie-consent module re-prompts every visitor whenever that version changes.
Both are driven by a **single constant** so they can never drift apart.

## Single source of truth

`client/src/lib/legal/legal-version.ts`

```ts
export const LEGAL_VERSION = "2026-05"; // YYYY-MM
```

- The legal pages render the human-readable label (`May 2026`) from this value
  as their "Last updated" stamp.
- `client/src/lib/consent/consent-record.ts` re-exports it as
  `CONSENT_POLICY_VERSION`. A stored consent record whose `policyVersion` no
  longer matches is treated as "no decision", so the banner reappears.

## Procedure

When you make a material change to the Privacy Policy or Terms of Service:

1. **Edit the policy text** in `client/src/components/legal/privacy-policy.tsx`
   and/or `client/src/components/legal/terms-of-service.tsx`.
2. **Bump `LEGAL_VERSION`** in `client/src/lib/legal/legal-version.ts` to the
   new `YYYY-MM` (e.g. `"2026-09"`). This is the only value you change.
3. **Run the checks**: `cd client && pnpm typecheck && pnpm test`. The
   legal-version tests assert the label format and that consent tracks the same
   value.
4. **Commit** with a conventional message, e.g.
   `feat(legal): update privacy policy and bump legal version to 2026-09`.
5. **Deploy.** On their next visit, every user is re-prompted for cookie consent
   automatically, because their stored `policyVersion` no longer matches.

No database migration or backend change is required — consent lives in a cookie
and the version comparison is purely client/BFF side.

> Only bump the version for **material** changes that warrant fresh consent.
> Typo fixes and cosmetic edits do not need a bump.
