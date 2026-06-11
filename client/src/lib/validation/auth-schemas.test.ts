import { describe, expect, it } from "vitest";
import { signupSchema } from "./auth-schemas";

const validSignup = {
  firstName: "Ada",
  lastName: "Lovelace",
  email: "ada@example.com",
  password: "Password1",
  confirmPassword: "Password1",
  acceptedTerms: true,
};

describe("signupSchema acceptedTerms", () => {
  it("accepts the form when the terms checkbox is ticked", () => {
    expect(signupSchema.safeParse(validSignup).success).toBe(true);
  });

  it("rejects the form when the terms checkbox is unticked", () => {
    const result = signupSchema.safeParse({
      ...validSignup,
      acceptedTerms: false,
    });
    expect(result.success).toBe(false);
  });

  it("surfaces a message on the acceptedTerms field when unticked", () => {
    const result = signupSchema.shape.acceptedTerms.safeParse(false);
    expect(result.success).toBe(false);
    expect(result.error?.issues[0]?.message).toMatch(/agree|accept|terms/i);
  });
});
