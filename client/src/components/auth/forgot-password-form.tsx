import { useState } from "react";
import { useForm } from "@tanstack/react-form";
import { Alert, Anchor, Button, Stack, Text, TextInput } from "@mantine/core";
import { Check } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { AuthFormWrapper } from "./auth-form-wrapper";
import type { ForgotPasswordFormData } from "@/lib/validation/auth-schemas";
import { forgotPasswordSchema } from "@/lib/validation/auth-schemas";
import { useAuth } from "@/hooks/use-auth";

interface ForgotPasswordFormProps {
  mode?: "modal" | "page";
  onSwitchToLogin?: () => void;
}

export function ForgotPasswordForm({
  mode = "page",
  onSwitchToLogin,
}: ForgotPasswordFormProps) {
  const { requestPasswordReset, isRequestingReset } = useAuth();
  const navigate = useNavigate();
  const [submitted, setSubmitted] = useState(false);

  const form = useForm({
    defaultValues: {
      email: "",
    },
    validators: {
      onChange: forgotPasswordSchema,
    },
    onSubmit: async ({ value }) => {
      const result = await requestPasswordReset(value.email);
      if (result.success) {
        setSubmitted(true);
      }
    },
  });

  const handleBackToLogin = () => {
    if (mode === "modal" && onSwitchToLogin) {
      onSwitchToLogin();
    } else {
      navigate({ to: "/login" });
    }
  };

  if (submitted) {
    return (
      <AuthFormWrapper title="Check your email">
        <Stack gap="md">
          <Alert color="green" icon={<Check size={16} />}>
            We've sent a password reset link to your email address. Please check
            your inbox and follow the instructions.
          </Alert>

          <Text size="sm" c="dimmed" ta="center">
            Didn't receive the email? Check your spam folder or try again.
          </Text>

          <Button variant="default" fullWidth onClick={handleBackToLogin}>
            Back to sign in
          </Button>
        </Stack>
      </AuthFormWrapper>
    );
  }

  return (
    <AuthFormWrapper
      title="Reset your password"
      subtitle="Enter your email address and we'll send you a reset link"
      footer={
        <>
          Remember your password?{" "}
          <Anchor component="button" onClick={handleBackToLogin}>
            Sign in
          </Anchor>
        </>
      }
    >
      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
        }}
      >
        <Stack gap="md">
          <form.Field
            name="email"
            validators={{
              onChange: forgotPasswordSchema.shape.email,
            }}
          >
            {(field) => (
              <TextInput
                label="Email"
                placeholder="your@email.com"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                error={
                  field.state.meta.isTouched &&
                  field.state.meta.errors.length > 0
                    ? field.state.meta.errors.map((e) => e.message).join(", ")
                    : undefined
                }
                disabled={isRequestingReset}
                required
              />
            )}
          </form.Field>

          <Button type="submit" fullWidth loading={isRequestingReset}>
            Send reset link
          </Button>
        </Stack>
      </form>
    </AuthFormWrapper>
  );
}
