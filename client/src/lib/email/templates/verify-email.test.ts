import { describe, expect, it } from "vitest";
import { buildVerifyEmailEmail } from "./verify-email";

describe("buildVerifyEmailEmail", () => {
  const baseInput = {
    name: "Anders Andersson",
    verifyUrl: "https://paddokk.com/verify-email?token=abc123",
  };

  it("returns a subject, html, and text body", () => {
    const result = buildVerifyEmailEmail(baseInput);
    expect(result.subject).toBeTypeOf("string");
    expect(result.subject.length).toBeGreaterThan(0);
    expect(result.html).toBeTypeOf("string");
    expect(result.html.length).toBeGreaterThan(0);
    expect(result.text).toBeTypeOf("string");
    expect(result.text.length).toBeGreaterThan(0);
  });

  it("subject mentions email verification", () => {
    const { subject } = buildVerifyEmailEmail(baseInput);
    expect(subject.toLowerCase()).toMatch(/verify|confirm/);
  });

  it("html body contains the verify URL", () => {
    const { html } = buildVerifyEmailEmail(baseInput);
    expect(html).toContain(baseInput.verifyUrl);
  });

  it("text body contains the verify URL", () => {
    const { text } = buildVerifyEmailEmail(baseInput);
    expect(text).toContain(baseInput.verifyUrl);
  });

  it("greets the user by name when provided", () => {
    const { html, text } = buildVerifyEmailEmail(baseInput);
    expect(html).toContain(baseInput.name);
    expect(text).toContain(baseInput.name);
  });

  it("falls back gracefully when name is empty", () => {
    const { html, text } = buildVerifyEmailEmail({
      ...baseInput,
      name: "",
    });
    expect(html).not.toContain("undefined");
    expect(html).not.toContain("null");
    expect(text).not.toContain("undefined");
    expect(text).not.toContain("null");
  });

  it("escapes HTML in the name to prevent injection", () => {
    const { html } = buildVerifyEmailEmail({
      ...baseInput,
      name: "<script>alert('xss')</script>",
    });
    expect(html).not.toContain("<script>");
    expect(html).not.toContain("alert('xss')");
  });
});
