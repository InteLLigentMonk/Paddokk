import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Alert, Button, Loader, Stack } from "@mantine/core";
import { AlertCircle, Check } from "lucide-react";
import { z } from "zod";
import { AuthFormWrapper } from "@/components/auth";
import { verifyEmail } from "@/lib/auth-client";

const verifyEmailSearchSchema = z.object({
  token: z.string().min(1, "Verification token is required"),
});

export const Route = createFileRoute("/_auth/verify-email")({
  validateSearch: verifyEmailSearchSchema,
  component: VerifyEmailPage,
});

function VerifyEmailPage() {
  const { token } = Route.useSearch();
  const navigate = useNavigate();
  const [status, setStatus] = useState<"loading" | "success" | "error">(
    "loading",
  );
  const [errorMessage, setErrorMessage] = useState<string>("");

  useEffect(() => {
    if (!token) {
      setStatus("error");
      setErrorMessage("Invalid verification link");
      return;
    }

    const verify = async () => {
      try {
        const result = await verifyEmail({
          query: { token },
        });

        if (result.error) {
          setStatus("error");
          setErrorMessage(
            result.error.message ||
              "Failed to verify email. The link may have expired.",
          );
        } else {
          setStatus("success");
        }
      } catch (error) {
        setStatus("error");
        setErrorMessage("An unexpected error occurred");
      }
    };

    verify();
  }, [token]);

  return (
    <AuthFormWrapper title="Email Verification">
      <Stack gap="md">
        {status === "loading" && (
          <Stack align="center" gap="md">
            <Loader size="lg" />
            <div>Verifying your email...</div>
          </Stack>
        )}

        {status === "success" && (
          <>
            <Alert color="green" icon={<Check size={16} />}>
              Your email has been verified successfully! You can now sign in to
              your account.
            </Alert>
            <Button fullWidth onClick={() => navigate({ to: "/login" })}>
              Continue to sign in
            </Button>
          </>
        )}

        {status === "error" && (
          <>
            <Alert color="red" icon={<AlertCircle size={16} />}>
              {errorMessage}
            </Alert>
            <Button
              variant="default"
              fullWidth
              onClick={() => navigate({ to: "/signup" })}
            >
              Back to sign up
            </Button>
          </>
        )}
      </Stack>
    </AuthFormWrapper>
  );
}
