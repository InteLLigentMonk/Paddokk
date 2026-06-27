# The error envelope's `message` is diagnostic; the frontend owns all user-facing copy, keyed by `code`

Every API failure returns the same envelope — `ApiErrorResponse(Code, Message, Status, Errors?)` (see [ApiErrorResponse.cs](../../API/Paddokk.Api/OpenApi/ApiErrorResponse.cs)) — produced either by a domain `Result` via `ApiControllerBase.FromError` or by an unhandled exception via `GlobalExceptionMiddleware`. This ADR fixes **what each field is for, where a failure is surfaced, and what is logged versus returned.** It supersedes the ad-hoc per-call-site handling that grew up around the error-code resolver (#278).

## `message` is for developers, never shown to users

The backend's `message` is **diagnostic English for logs and developers**. The frontend never renders it. All user-facing copy lives on the client, selected by `code` through the error-code resolver ([error-resolver.ts](../../client/src/lib/api/error-resolver.ts)).

This dissolves two problems at once:

- **Leakage.** Once `message` is never shown, it does not matter that some exception branches were echoing `ex.Message` to the client. We stop returning raw exception text for `ArgumentException` / `InvalidOperationException` / `KeyNotFoundException` / `UnauthorizedAccessException` regardless — but the trust boundary no longer depends on remembering to.
- **Inconsistent copy.** Surfaces like the change-username form and the export action were each inventing their own copy (and in one case showing the raw backend string). With copy keyed on `code` in one table, a given failure reads identically everywhere — toast, inline alert, or boundary.

The cost is granularity: any failure that needs *specific, actionable* wording must carry a *specific code*. We accept that — see below.

## A code per actionable case; generic fallback for the rest

`ErrorType` → status mapping and the generic codes (`NOT_FOUND`, `CONFLICT`, `FORBIDDEN`, `VALIDATION_FAILED`, `INTERNAL`, `RATE_LIMITED`, `REQUEST_CANCELLED`) stay. On top of them we add **domain codes for every case the user can act on** — e.g. `USERNAME_TAKEN`, `USERNAME_CHANGE_TOO_SOON`, `EXPORT_COOLDOWN`. Codes are a permanent part of the API contract ([ErrorCodes.cs](../../API/Paddokk.Core/Models/ErrorCodes.cs)) and must not change once shipped.

Anything without curated copy hits the resolver's generic fallback ("Something went wrong" + "Report a problem"). We do **not** chase a code for every conceivable failure — only for failures where generic copy would leave the user stuck.

## Three surfaces, one rule each

A failure is shown in exactly one place, decided by *whether the view can still function*:

- **Full-page boundary** — when the view cannot render: a route loader throws, or the resource the whole route is built on is missing. `NOT_FOUND` from a loader → `notFoundComponent`; anything else fatal → `errorComponent` ([error-page.tsx](../../client/src/components/common/error-page.tsx)). Route-critical queries stop blindly toasting `NOT_FOUND`; they bubble to the boundary.
- **Toast** — when the page stays up but an *action* failed: mutations (like, follow, delete) and non-critical widget/list queries. This is the default of the global query/mutation handlers ([error-handler.ts](../../client/src/integrations/tanstack-query/error-handler.ts)).
- **Inline** — when the failure belongs to a *specific form or control*: validation and state conflicts the user can fix in place (username taken, 30-day limit, export cooldown). Inline copy is resolved from the **same** `CODE_MESSAGES` table as toasts, so there is no second source of truth.

## Client-side validation is UX; server-side validation that the client cannot do is modelled as domain codes

Format/shape validation (length, regex, required) is enforced client-side with Zod for fast feedback, and re-enforced on the backend for security (OWASP — client validation is never the gate). In practice the backend rarely sees those failures, so we do **not** build generic `errors[]`-to-field binding for them; if one slips through, a generic toast is acceptable.

The validation the client *cannot* perform — uniqueness (`USERNAME_TAKEN`), state/race (`USERNAME_CHANGE_TOO_SOON`, `EXPORT_COOLDOWN`), cross-entity checks — is modelled as **domain codes shown inline**, not as generic field errors. The form at the call site knows which control a code belongs to. The `errors[]` array remains in the envelope for diagnostics and any future server-only multi-field case, but is not consumed for field binding until such a case actually exists.

## Logging versus returning, and a correlation id

- **Returned to the client:** `code`, a safe (diagnostic, never-rendered) `message`, `status`, optional `errors[]`, and a **correlation id**. Never a stack trace.
- **Logged on the server:** unhandled / `INTERNAL` failures at `Error` level with the full exception and the same correlation id, so a user's "Report a problem" can be matched to the log line. **Expected 4xx** — validation failures, domain conflicts — drop from `Error` to `Information`/`Warning`; they are not defects and should not page anyone.

The correlation id (request `TraceIdentifier` / `Activity`) is carried in the envelope and surfaced through the support link, closing the loop between a user report and the server log.

## Consequences

- A new ground rule for the frontend: **never render `error.message`.** Reviews reject `notifications.show({ message: error.message })` and any inline use of the backend string. Copy is added to `CODE_MESSAGES`, never to a call site.
- Adding a user-actionable failure is now a **two-sided change**: a code in `ErrorCodes.cs` (backend) and curated copy in the resolver (frontend). The OpenAPI/Orval regen does not carry copy — that is deliberate, copy is a frontend concern.
- The toggle mutations (like/subscribe) stop setting `meta.suppressGlobalError` and calling `notifyApiError` by hand; `onError` does rollback only and the global handler owns the toast. `suppressGlobalError` is reserved for mutations that genuinely render their error inline.
- `GlobalExceptionMiddleware` stops returning raw `ex.Message` for the typed exception branches and re-tiers its log levels. `FromError` continues not to log (domain 4xx are expected).
- If the product later needs localisation, copy already lives in one client-side table keyed by a stable code — the i18n seam is the resolver, not scattered call sites.
- Revisit if a real server-only **multi-field** validation case appears: that is the trigger to actually consume `errors[]` for field binding, which this ADR otherwise defers.
