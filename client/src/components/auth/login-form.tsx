import { useForm } from "@tanstack/react-form";
import { Anchor, Button, PasswordInput, Stack, TextInput } from "@mantine/core";
import { useNavigate } from "@tanstack/react-router";
import { SocialLoginButtons } from "./social-login-buttons";
import { AuthFormWrapper } from "./auth-form-wrapper";
import { loginSchema } from "@/lib/validation/auth-schemas";
import { useAuth } from "@/hooks/use-auth";

interface LoginFormProps {
  mode?: "modal" | "page";
  onSwitchToSignup?: () => void;
  onSwitchToForgotPassword?: () => void;
}

export function LoginForm({
  mode = "page",
  onSwitchToSignup,
  onSwitchToForgotPassword,
}: LoginFormProps) {
  const { login, isLoggingIn } = useAuth();
  const navigate = useNavigate();

  const form = useForm({
    defaultValues: {
      email: "",
      password: "",
    },
    validators: {
      onChange: loginSchema,
    },
    onSubmit: async ({ value }) => {
      await login(value.email, value.password);
    },
  });

  const handleSwitchToSignup = () => {
    if (mode === "modal" && onSwitchToSignup) {
      onSwitchToSignup();
    } else {
      navigate({ to: "/signup" });
    }
  };

  const handleSwitchToForgotPassword = () => {
    if (mode === "modal" && onSwitchToForgotPassword) {
      onSwitchToForgotPassword();
    } else {
      navigate({ to: "/forgot-password" });
    }
  };

  return (
    <AuthFormWrapper
      title="Sign in to Paddokk"
      footer={
        <>
          Don't have an account?{" "}
          <Anchor component="button" onClick={handleSwitchToSignup}>
            Sign up
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

          <form.Field
            name="email"
            validators={{
              onChange: loginSchema.shape.email,
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
                        .map((e) => e?.message || "")
                        .join(", ")
                    : undefined
                }
                disabled={isLoggingIn}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="password"
            validators={{
              onChange: loginSchema.shape.password,
            }}
          >
            {(field) => (
              <PasswordInput
                label="Password"
                placeholder="Your password"
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
                disabled={isLoggingIn}
                required
              />
            )}
          </form.Field>

          <Anchor
            component="button"
            type="button"
            size="sm"
            onClick={handleSwitchToForgotPassword}
            ta="right"
          >
            Forgot password?
          </Anchor>

          <Button type="submit" fullWidth loading={isLoggingIn}>
            Sign in
          </Button>
        </Stack>
      </form>
    </AuthFormWrapper>
  );
}
