import { describe, expect, it } from "vitest";
import { ApiError } from "./api-error";
import { requirePage } from "./infinite";

describe("requirePage", () => {
  it("resolves with the page when the response has a body", async () => {
    const page = { hasMore: false, items: [] };
    await expect(requirePage(Promise.resolve(page))).resolves.toBe(page);
  });

  it("rejects with an ApiError when the response is undefined (HTTP 204)", async () => {
    await expect(
      requirePage(Promise.resolve(undefined)),
    ).rejects.toBeInstanceOf(ApiError);
  });

  it("rejects with an ApiError when the response is null", async () => {
    await expect(requirePage(Promise.resolve(null))).rejects.toBeInstanceOf(
      ApiError,
    );
  });
});
