import { useForm } from "@tanstack/react-form";
import {
  Anchor,
  Button,
  Checkbox,
  Group,
  PasswordInput,
  Stack,
  TextInput,
} from "@mantine/core";
import { Link, useNavigate } from "@tanstack/react-router";
import { SocialLoginButtons } from "./social-login-buttons";
import { AuthFormWrapper } from "./auth-form-wrapper";
import { signupSchema } from "@/lib/validation/auth-schemas";
import { useAuth } from "@/hooks/use-auth";

interface SignupFormProps {
  mode?: "modal" | "page";
  onSwitchToLogin?: () => void;
}

export function SignupForm({
  mode = "page",
  onSwitchToLogin,
}: SignupFormProps) {
  const { register, isRegistering } = useAuth();
  const navigate = useNavigate();

  const form = useForm({
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
      acceptedTerms: false as boolean,
    },
    validators: {
      onChange: signupSchema,
    },
    onSubmit: async ({ value }) => {
      await register(
        value.firstName,
        value.lastName || undefined,
        value.email,
        value.password,
      );
    },
  });

  const handleSwitchToLogin = () => {
    if (mode === "modal" && onSwitchToLogin) {
      onSwitchToLogin();
    } else {
      navigate({ to: "/login" });
    }
  };

  return (
    <AuthFormWrapper
      title="Create your account"
      subtitle="Join thousands of car enthusiasts"
      footer={
        <>
          Already have an account?{" "}
          <Anchor component="button" onClick={handleSwitchToLogin}>
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
          <SocialLoginButtons />

          <Group grow align="flex-start">
            <form.Field
              name="firstName"
              validators={{
                onChange: signupSchema.shape.firstName,
              }}
            >
              {(field) => (
                <TextInput
                  label="First name"
                  placeholder="Your first name"
                  value={field.state.value}
                  onChange={(e) => field.handleChange(e.target.value)}
                  onBlur={field.handleBlur}
                  error={
                    field.state.meta.isTouched &&
                    field.state.meta.errors.length > 0
                      ? field.state.meta.errors
                          .map((e) => e?.message ?? "")
                          .join(", ")
                      : undefined
                  }
                  disabled={isRegistering}
                  required
                />
              )}
            </form.Field>

            <form.Field
              name="lastName"
              validators={{
                onChange: signupSchema.shape.lastName,
              }}
            >
              {(field) => (
                <TextInput
                  label="Last name"
                  placeholder="Your last name"
                  value={field.state.value}
                  onChange={(e) => field.handleChange(e.target.value)}
                  onBlur={field.handleBlur}
                  error={
                    field.state.meta.isTouched &&
                    field.state.meta.errors.length > 0
                      ? field.state.meta.errors
                          .map((e) => e?.message ?? "")
                          .join(", ")
                      : undefined
                  }
                  disabled={isRegistering}
                />
              )}
            </form.Field>
          </Group>

          <form.Field
            name="email"
            validators={{
              onChange: signupSchema.shape.email,
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
                    ? field.state.meta.errors
                        .map((e) => e?.message ?? "")
                        .join(", ")
                    : undefined
                }
                disabled={isRegistering}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="password"
            validators={{
              onChange: signupSchema.shape.password,
            }}
          >
            {(field) => (
              <PasswordInput
                label="Password"
                placeholder="Create a strong password"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                error={
                  field.state.meta.isTouched &&
                  field.state.meta.errors.length > 0
                    ? field.state.meta.errors
                        .map((e) => e?.message ?? "")
                        .join(", ")
                    : undefined
                }
                disabled={isRegistering}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="confirmPassword"
            validators={{
              onChange: signupSchema.shape.confirmPassword,
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
                        .map((e) => e?.message ?? "")
                        .join(", ")
                    : undefined
                }
                disabled={isRegistering}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="acceptedTerms"
            validators={{
              onChange: signupSchema.shape.acceptedTerms,
            }}
          >
            {(field) => (
              <Checkbox
                checked={field.state.value}
                onChange={(e) => field.handleChange(e.currentTarget.checked)}
                onBlur={field.handleBlur}
                disabled={isRegistering}
                error={
                  field.state.meta.isTouched &&
                  field.state.meta.errors.length > 0
                    ? field.state.meta.errors
                        .map((e) => e?.message ?? "")
                        .join(", ")
                    : undefined
                }
                label={
                  <>
                    I have read and agree to the{" "}
                    <Anchor
                      component={Link}
                      to="/privacy"
                      target="_blank"
                      rel="noopener noreferrer"
                      inherit
                    >
                      Privacy Policy
                    </Anchor>{" "}
                    and{" "}
                    <Anchor
                      component={Link}
                      to="/terms"
                      target="_blank"
                      rel="noopener noreferrer"
                      inherit
                    >
                      Terms of Service
                    </Anchor>
                  </>
                }
              />
            )}
          </form.Field>

          <Button type="submit" fullWidth loading={isRegistering}>
            Create account
          </Button>
        </Stack>
      </form>
    </AuthFormWrapper>
  );
}
