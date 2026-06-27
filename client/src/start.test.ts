import { describe, expect, it } from "vitest";
import { startInstance } from "./start";
import { apiErrorSerializationAdapter } from "./lib/api/api-error-serialization";
import { ApiError } from "./lib/api/api-error";

/**
 * Locks the wiring that silently regressed once: `api-error-serialization.ts` existed but
 * was never registered, so the Start runtime fell back to the built-in `$TSR/Error` plugin
 * and only `message` survived the BFF boundary — degrading every API failure to the generic
 * fallback toast (see ADR-0007). These tests fail if the adapter is dropped from
 * `src/start.ts`, or if the adapter stops round-tripping the full envelope.
 */
describe("startInstance serialization wiring", () => {
  it("registers the ApiError serialization adapter", async () => {
    const options = await startInstance.getOptions();
    expect(options.serializationAdapters).toContain(
      apiErrorSerializationAdapter,
    );
  });

  it("round-trips the full ApiError envelope (code/status/errors/traceId)", () => {
    const original = new ApiError(
      409,
      "Cannot like your own journey",
      "LIKE_OWN_SUBJECT",
      [{ field: "journeyId", code: "CONFLICT", message: "own" }],
      "trace-123",
    );

    // The adapter is what the Start runtime applies on each side of the network boundary.
    const restored = apiErrorSerializationAdapter.fromSerializable(
      apiErrorSerializationAdapter.toSerializable(original),
    );

    expect(restored).toBeInstanceOf(ApiError);
    expect(restored.status).toBe(409);
    expect(restored.code).toBe("LIKE_OWN_SUBJECT");
    expect(restored.traceId).toBe("trace-123");
    expect(restored.errors).toEqual([
      { field: "journeyId", code: "CONFLICT", message: "own" },
    ]);
  });
});
