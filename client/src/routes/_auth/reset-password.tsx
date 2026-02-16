import { createFileRoute } from "@tanstack/react-router";
import { Alert, Stack } from "@mantine/core";
import { AlertCircle } from "lucide-react";
import { z } from "zod";
import { AuthFormWrapper, ResetPasswordForm } from "@/components/auth";

const resetPasswordSearchSchema = z.object({
  token: z.string().min(1, "Reset token is required"),
});

export const Route = createFileRoute("/_auth/reset-password")({
  validateSearch: resetPasswordSearchSchema,
  component: ResetPasswordPage,
});

function ResetPasswordPage() {
  const { token } = Route.useSearch();

  if (!token) {
    return (
      <AuthFormWrapper title="Invalid Reset Link">
        <Stack gap="md">
          <Alert color="red" icon={<AlertCircle size={16} />}>
            This password reset link is invalid or has expired. Please request a
            new one.
          </Alert>
        </Stack>
      </AuthFormWrapper>
    );
  }

  return <ResetPasswordForm token={token} />;
}
