import { describe, expect, it, vi } from "vitest";
import { deleteApiUser } from "./delete-api-user";

describe("deleteApiUser", () => {
  const apiUrl = "https://api.example.com";

  it("throws without calling the API when the token is missing", async () => {
    const fetchImpl = vi.fn();

    await expect(
      deleteApiUser({ apiUrl, token: null, fetchImpl }),
    ).rejects.toThrow();
    expect(fetchImpl).not.toHaveBeenCalled();
  });

  it("throws when the API responds with a non-ok status", async () => {
    const fetchImpl = vi
      .fn()
      .mockResolvedValue(new Response(null, { status: 500 }));

    await expect(
      deleteApiUser({ apiUrl, token: "tok", fetchImpl }),
    ).rejects.toThrow();
  });

  it("issues a bearer DELETE to the users endpoint on success", async () => {
    const fetchImpl = vi
      .fn()
      .mockResolvedValue(new Response(null, { status: 200 }));

    await deleteApiUser({ apiUrl, token: "tok", fetchImpl });

    expect(fetchImpl).toHaveBeenCalledWith(
      "https://api.example.com/api/v1/Users/me",
      { method: "DELETE", headers: { Authorization: "Bearer tok" } },
    );
  });
});
