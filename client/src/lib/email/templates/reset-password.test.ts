import { describe, expect, it } from "vitest";
import { buildResetPasswordEmail } from "./reset-password";

describe("buildResetPasswordEmail", () => {
  const baseInput = {
    name: "Anders Andersson",
    resetUrl: "https://paddokk.com/reset-password?token=abc123",
  };

  it("returns a subject, html, and text body", () => {
    const result = buildResetPasswordEmail(baseInput);
    expect(result.subject).toBeTypeOf("string");
    expect(result.subject.length).toBeGreaterThan(0);
    expect(result.html).toBeTypeOf("string");
    expect(result.html.length).toBeGreaterThan(0);
    expect(result.text).toBeTypeOf("string");
    expect(result.text.length).toBeGreaterThan(0);
  });

  it("subject mentions password reset", () => {
    const { subject } = buildResetPasswordEmail(baseInput);
    expect(subject.toLowerCase()).toContain("password");
  });

  it("html body contains the reset URL", () => {
    const { html } = buildResetPasswordEmail(baseInput);
    expect(html).toContain(baseInput.resetUrl);
  });

  it("text body contains the reset URL", () => {
    const { text } = buildResetPasswordEmail(baseInput);
    expect(text).toContain(baseInput.resetUrl);
  });

  it("greets the user by name when provided", () => {
    const { html, text } = buildResetPasswordEmail(baseInput);
    expect(html).toContain(baseInput.name);
    expect(text).toContain(baseInput.name);
  });

  it("falls back gracefully when name is empty", () => {
    const { html, text } = buildResetPasswordEmail({
      ...baseInput,
      name: "",
    });
    expect(html).not.toContain("undefined");
    expect(html).not.toContain("null");
    expect(text).not.toContain("undefined");
    expect(text).not.toContain("null");
  });

  it("escapes HTML in the name to prevent injection", () => {
    const { html } = buildResetPasswordEmail({
      ...baseInput,
      name: "<script>alert('xss')</script>",
    });
    expect(html).not.toContain("<script>");
    expect(html).not.toContain("alert('xss')");
  });
});
