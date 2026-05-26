import { useState } from "react";
import { useForm } from "@tanstack/react-form";
import { Button, Checkbox, Group, PasswordInput, Stack } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { changePassword } from "@/lib/auth-client";
import { changePasswordSchema } from "@/lib/validation/profile-schemas";

export function ChangePasswordForm() {
  const [isPending, setIsPending] = useState(false);

  const form = useForm({
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
      revokeOtherSessions: true,
    },
    validators: { onChange: changePasswordSchema },
    onSubmit: async ({ value }) => {
      setIsPending(true);
      try {
        const { error } = await changePassword({
          currentPassword: value.currentPassword,
          newPassword: value.newPassword,
          revokeOtherSessions: value.revokeOtherSessions,
        });
        if (error) {
          notifications.show({
            color: "red",
            message: error.message ?? "Could not change password",
          });
          return;
        }
        notifications.show({
          color: "green",
          message: value.revokeOtherSessions
            ? "Password updated. Other sessions have been signed out."
            : "Password updated.",
        });
        form.reset();
      } catch {
        notifications.show({
          color: "red",
          message: "Could not change password. Please try again.",
        });
      } finally {
        setIsPending(false);
      }
    },
  });

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        form.handleSubmit();
      }}
    >
      <Stack gap="md">
        <form.Field name="currentPassword">
          {(field) => (
            <PasswordInput
              label="Current password"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              onBlur={field.handleBlur}
              error={
                field.state.meta.isTouched && field.state.meta.errors.length > 0
                  ? field.state.meta.errors
                      .map((e) => e?.message ?? "")
                      .join(", ")
                  : undefined
              }
              disabled={isPending}
              required
              autoComplete="current-password"
            />
          )}
        </form.Field>

        <form.Field name="newPassword">
          {(field) => (
            <PasswordInput
              label="New password"
              description="At least 8 characters with upper, lower and a digit"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              onBlur={field.handleBlur}
              error={
                field.state.meta.isTouched && field.state.meta.errors.length > 0
                  ? field.state.meta.errors
                      .map((e) => e?.message ?? "")
                      .join(", ")
                  : undefined
              }
              disabled={isPending}
              required
              autoComplete="new-password"
            />
          )}
        </form.Field>

        <form.Field name="confirmPassword">
          {(field) => (
            <PasswordInput
              label="Confirm new password"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              onBlur={field.handleBlur}
              error={
                field.state.meta.isTouched && field.state.meta.errors.length > 0
                  ? field.state.meta.errors
                      .map((e) => e?.message ?? "")
                      .join(", ")
                  : undefined
              }
              disabled={isPending}
              required
              autoComplete="new-password"
            />
          )}
        </form.Field>

        <form.Field name="revokeOtherSessions">
          {(field) => (
            <Checkbox
              label="Sign out other devices"
              description="Recommended after a password change"
              checked={field.state.value}
              onChange={(e) => field.handleChange(e.currentTarget.checked)}
              disabled={isPending}
            />
          )}
        </form.Field>

        <Group justify="flex-end">
          <form.Subscribe selector={(state) => state.canSubmit}>
            {(canSubmit) => (
              <Button type="submit" loading={isPending} disabled={!canSubmit}>
                Update password
              </Button>
            )}
          </form.Subscribe>
        </Group>
      </Stack>
    </form>
  );
}
