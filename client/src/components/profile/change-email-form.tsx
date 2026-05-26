import { useState } from "react";
import { useForm } from "@tanstack/react-form";
import { Alert, Button, Group, Stack, Text, TextInput } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { Info, MailCheck } from "lucide-react";
import type { UserDto } from "@/generated/api/schemas";
import { changeEmail } from "@/lib/auth-client";
import { changeEmailSchema } from "@/lib/validation/profile-schemas";

interface ChangeEmailFormProps {
  user: UserDto;
}

export function ChangeEmailForm({ user }: ChangeEmailFormProps) {
  const [isPending, setIsPending] = useState(false);
  const [pendingEmail, setPendingEmail] = useState<string | null>(null);

  const form = useForm({
    defaultValues: { newEmail: "" },
    validators: { onChange: changeEmailSchema },
    onSubmit: async ({ value }) => {
      if (value.newEmail === user.email) {
        notifications.show({
          color: "yellow",
          message: "That's already your current email.",
        });
        return;
      }
      setIsPending(true);
      try {
        const callbackURL = `${window.location.origin}/me/settings`;
        const { error } = await changeEmail({
          newEmail: value.newEmail,
          callbackURL,
        });
        if (error) {
          notifications.show({
            color: "red",
            message: error.message ?? "Could not request email change",
          });
          return;
        }
        setPendingEmail(value.newEmail);
        form.reset();
      } catch {
        notifications.show({
          color: "red",
          message: "Could not request email change. Please try again.",
        });
      } finally {
        setIsPending(false);
      }
    },
  });

  return (
    <Stack gap="md">
      <Alert icon={<Info size={16} />} color="blue" variant="light">
        <Text size="sm">
          Your current email is <strong>{user.email}</strong>. A confirmation
          link will be sent to your new address. Your email won't change until
          you click the link.
        </Text>
      </Alert>

      {pendingEmail ? (
        <Alert icon={<MailCheck size={16} />} color="green" variant="light">
          <Text size="sm">
            We sent a confirmation link to <strong>{pendingEmail}</strong>. Open
            it from that inbox to finish the change.
          </Text>
        </Alert>
      ) : null}

      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
        }}
      >
        <Stack gap="md">
          <form.Field name="newEmail">
            {(field) => (
              <TextInput
                label="New email"
                placeholder="you@example.com"
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
                disabled={isPending}
                required
                autoComplete="email"
              />
            )}
          </form.Field>

          <Group justify="flex-end">
            <form.Subscribe selector={(state) => state.canSubmit}>
              {(canSubmit) => (
                <Button type="submit" loading={isPending} disabled={!canSubmit}>
                  Send confirmation
                </Button>
              )}
            </form.Subscribe>
          </Group>
        </Stack>
      </form>
    </Stack>
  );
}
