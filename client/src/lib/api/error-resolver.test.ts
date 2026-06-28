import { describe, expect, it } from "vitest";
import { ApiError } from "./api-error";
import {
  ApiErrorCode,
  FALLBACK_MESSAGE,
  RATE_LIMIT_MESSAGE,
  isNetworkError,
  isNotFoundError,
  resolveApiError,
} from "./error-resolver";

describe("resolveApiError", () => {
  it("maps a known domain code to its curated message", () => {
    const error = new ApiError(
      409,
      "backend copy",
      ApiErrorCode.SubscribeToOwnSubject,
    );
    expect(resolveApiError(error)).toEqual({
      message: "You can't follow your own journey or car.",
      severity: "error",
    });
  });

  it("maps the like-own-subject code to its curated message", () => {
    const error = new ApiError(
      409,
      "Cannot like your own journey",
      ApiErrorCode.LikeOwnSubject,
    );
    expect(resolveApiError(error)).toEqual({
      message: "You can't like your own journey or car.",
      severity: "error",
    });
  });

  it("treats a 429 status as a rate-limit warning", () => {
    const error = new ApiError(429, "Too many", ApiErrorCode.RateLimited);
    expect(resolveApiError(error)).toEqual({
      message: RATE_LIMIT_MESSAGE,
      severity: "warning",
    });
  });

  it("treats a non-429 RATE_LIMITED code as a rate-limit warning", () => {
    const error = new ApiError(503, "slow down", ApiErrorCode.RateLimited);
    expect(resolveApiError(error)?.severity).toBe("warning");
  });

  it("returns null for an aborted request so nothing is shown", () => {
    const error = new Error("aborted");
    error.name = "AbortError";
    expect(resolveApiError(error)).toBeNull();
  });

  it("returns null for a server-side request-cancelled code", () => {
    const error = new ApiError(499, "cancelled", ApiErrorCode.RequestCancelled);
    expect(resolveApiError(error)).toBeNull();
  });

  it("falls back to a generic message with a report CTA for unmapped codes", () => {
    const error = new ApiError(500, "boom", "SOME_UNKNOWN_CODE");
    const resolved = resolveApiError(error);
    expect(resolved?.message).toBe(FALLBACK_MESSAGE);
    expect(resolved?.severity).toBe("error");
    expect(resolved?.cta?.label).toBe("Report a problem");
    expect(resolved?.cta?.href).toContain("mailto:");
  });

  it("uses a caller-supplied fallback message without a report CTA", () => {
    const error = new ApiError(400, "boom", "SOME_UNKNOWN_CODE");
    expect(
      resolveApiError(error, { fallbackMessage: "Upload failed." }),
    ).toEqual({ message: "Upload failed.", severity: "error" });
  });

  it("maps upload validator codes regardless of a caller fallback", () => {
    const error = new ApiError(400, "too big", ApiErrorCode.UploadTooLarge);
    const resolved = resolveApiError(error, {
      fallbackMessage: "Upload failed.",
    });
    expect(resolved?.message).toContain("too large");
    expect(resolved?.severity).toBe("error");
  });

  it("maps UPLOAD_TOO_LARGE to image-specific copy with the size limit", () => {
    const error = new ApiError(400, "too big", ApiErrorCode.UploadTooLarge);
    expect(resolveApiError(error)).toEqual({
      message: "Your image is too large. Maximum size is 5 MB.",
      severity: "error",
    });
  });

  it("maps UPLOAD_UNSUPPORTED_FORMAT to the supported-formats copy", () => {
    const error = new ApiError(
      400,
      "bad type",
      ApiErrorCode.UploadUnsupportedFormat,
    );
    expect(resolveApiError(error)).toEqual({
      message: "Only JPEG, PNG, and WebP images are supported.",
      severity: "error",
    });
  });

  it("maps UPLOAD_DIMENSIONS_TOO_SMALL to actionable copy", () => {
    const error = new ApiError(
      400,
      "too small",
      ApiErrorCode.UploadDimensionsTooSmall,
    );
    expect(resolveApiError(error)?.message).toContain("too small");
  });

  it("maps UPLOAD_DIMENSIONS_TOO_LARGE to actionable copy", () => {
    const error = new ApiError(
      400,
      "too big",
      ApiErrorCode.UploadDimensionsTooLarge,
    );
    expect(resolveApiError(error)?.message).toContain("4000×4000");
  });

  it("maps UPLOAD_INVALID_IMAGE to a not-a-valid-image message", () => {
    const error = new ApiError(
      400,
      "undecodable",
      ApiErrorCode.UploadInvalidImage,
    );
    expect(resolveApiError(error)?.message).toContain("valid image");
  });

  it("falls back for a non-ApiError thrown value", () => {
    expect(resolveApiError(new Error("weird"))?.message).toBe(FALLBACK_MESSAGE);
  });

  // Errors thrown by apiFetcher inside a BFF server function are serialized by
  // seroval, which reconstructs them on the client as a plain Error (losing the
  // ApiError prototype) while preserving name/code/status/errors. resolveApiError
  // must still recognise the code from this deserialized shape.
  it("maps a code from an error deserialized across the server-fn boundary", () => {
    const deserialized = Object.assign(new Error("backend copy"), {
      name: "ApiError",
      code: ApiErrorCode.SubscribeToOwnSubject,
      status: 409,
    });
    expect(resolveApiError(deserialized)).toEqual({
      message: "You can't follow your own journey or car.",
      severity: "error",
    });
  });

  it("treats a deserialized 429 as a rate-limit warning", () => {
    const deserialized = Object.assign(new Error("Too many"), {
      name: "ApiError",
      code: ApiErrorCode.RateLimited,
      status: 429,
    });
    expect(resolveApiError(deserialized)?.severity).toBe("warning");
  });

  it("maps the username-taken code as an error", () => {
    const error = new ApiError(409, "taken", ApiErrorCode.UsernameTaken);
    expect(resolveApiError(error)).toEqual({
      message: "That username is already taken. Please choose another.",
      severity: "error",
    });
  });

  it("maps cooldown codes as soft warnings", () => {
    const tooSoon = new ApiError(
      409,
      "wait",
      ApiErrorCode.UsernameChangeTooSoon,
    );
    const exportCooldown = new ApiError(
      409,
      "wait",
      ApiErrorCode.ExportCooldown,
    );
    expect(resolveApiError(tooSoon)?.severity).toBe("warning");
    expect(resolveApiError(exportCooldown)?.severity).toBe("warning");
  });
});

describe("isNetworkError", () => {
  it("is true for a connection failure (fetch TypeError)", () => {
    expect(isNetworkError(new TypeError("Failed to fetch"))).toBe(true);
  });

  it("is false for a structured ApiError", () => {
    expect(
      isNetworkError(new ApiError(500, "boom", ApiErrorCode.Internal)),
    ).toBe(false);
  });

  it("is false for an aborted request (user/navigation initiated)", () => {
    const error = new Error("aborted");
    error.name = "AbortError";
    expect(isNetworkError(error)).toBe(false);
  });

  it("is false for a non-Error thrown value", () => {
    expect(isNetworkError("nope")).toBe(false);
  });
});

describe("isNotFoundError", () => {
  it("is true for a 404 status", () => {
    expect(isNotFoundError(new ApiError(404, "gone", "WHATEVER"))).toBe(true);
  });

  it("is true for the NOT_FOUND code on a non-404 status", () => {
    expect(
      isNotFoundError(new ApiError(400, "gone", ApiErrorCode.NotFound)),
    ).toBe(true);
  });

  it("is false for other API errors", () => {
    expect(
      isNotFoundError(new ApiError(500, "boom", ApiErrorCode.Internal)),
    ).toBe(false);
  });

  it("is false for non-API errors", () => {
    expect(isNotFoundError(new Error("weird"))).toBe(false);
  });
});
