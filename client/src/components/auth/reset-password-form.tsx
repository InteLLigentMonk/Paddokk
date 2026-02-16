import { useForm } from "@tanstack/react-form";
import { Button, PasswordInput, Stack } from "@mantine/core";
import { AuthFormWrapper } from "./auth-form-wrapper";
import { resetPasswordSchema } from "@/lib/validation/auth-schemas";
import { useAuth } from "@/hooks/use-auth";

interface ResetPasswordFormProps {
  token: string;
}

export function ResetPasswordForm({ token }: ResetPasswordFormProps) {
  const { resetPasswordWithToken, isResettingPassword } = useAuth();

  const form = useForm({
    defaultValues: {
      password: "",
      confirmPassword: "",
    },
    validators: {
      onChange: resetPasswordSchema,
    },
    onSubmit: async ({ value }) => {
      await resetPasswordWithToken(token, value.password);
    },
  });

  return (
    <AuthFormWrapper
      title="Create a new password"
      subtitle="Choose a strong password for your account"
    >
      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
        }}
      >
        <Stack gap="md">
          <form.Field
            name="password"
            validators={{
              onChange: resetPasswordSchema.shape.password,
            }}
          >
            {(field) => (
              <PasswordInput
                label="New Password"
                placeholder="Create a strong password"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                error={
                  field.state.meta.isTouched &&
                  field.state.meta.errors.length > 0
                    ? field.state.meta.errors
                        .map((e) => e?.message || "")
                        .join(", ")
                    : undefined
                }
                disabled={isResettingPassword}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="confirmPassword"
            validators={{
              onChange: resetPasswordSchema.shape.confirmPassword,
            }}
          >
            {(field) => (
              <PasswordInput
                label="Confirm Password"
                placeholder="Confirm your password"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                error={
                  field.state.meta.isTouched &&
                  field.state.meta.errors.length > 0
                    ? field.state.meta.errors
                        .map((e) => e?.message || "")
                        .join(", ")
                    : undefined
                }
                disabled={isResettingPassword}
                required
              />
            )}
          </form.Field>

          <Button type="submit" fullWidth loading={isResettingPassword}>
            Reset password
          </Button>
        </Stack>
      </form>
    </AuthFormWrapper>
  );
}
