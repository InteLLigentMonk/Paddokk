import { useState } from "react";
import { useForm } from "@tanstack/react-form";
import {
  Alert,
  Button,
  Group,
  Modal,
  PasswordInput,
  Stack,
  Text,
  TextInput,
} from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { useNavigate } from "@tanstack/react-router";
import { AlertTriangle } from "lucide-react";
import { deleteUser } from "@/lib/auth-client";
import { deleteAccountSchema } from "@/lib/validation/profile-schemas";

interface DeleteAccountModalProps {
  opened: boolean;
  onClose: () => void;
}

export function DeleteAccountModal({
  opened,
  onClose,
}: DeleteAccountModalProps) {
  const navigate = useNavigate();
  const [isPending, setIsPending] = useState(false);

  const form = useForm({
    defaultValues: { confirmation: "", password: "" },
    validators: { onChange: deleteAccountSchema },
    onSubmit: async ({ value }) => {
      setIsPending(true);
      try {
        const { error } = await deleteUser({ password: value.password });
        if (error) {
          notifications.show({
            color: "red",
            message: error.message ?? "Could not delete account",
          });
          return;
        }
        notifications.show({
          color: "blue",
          message: "Your account has been deleted.",
        });
        navigate({ to: "/" });
      } catch {
        notifications.show({
          color: "red",
          message: "Could not delete account. Please try again.",
        });
      } finally {
        setIsPending(false);
      }
    },
  });

  const handleClose = () => {
    if (isPending) return;
    form.reset();
    onClose();
  };

  return (
    <Modal
      opened={opened}
      onClose={handleClose}
      title="Delete account"
      centered
      size="md"
      closeOnClickOutside={!isPending}
      closeOnEscape={!isPending}
    >
      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
        }}
      >
        <Stack gap="md">
          <Alert
            color="red"
            variant="light"
            icon={<AlertTriangle size={16} />}
          >
            <Text size="sm">
              This will permanently anonymize your profile and sign you out.
              Your cars, journeys and posts will remain visible but attributed
              to "Deleted User". This action cannot be undone.
            </Text>
          </Alert>

          <form.Field name="confirmation">
            {(field) => (
              <TextInput
                label="Type DELETE to confirm"
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
                autoComplete="off"
              />
            )}
          </form.Field>

          <form.Field name="password">
            {(field) => (
              <PasswordInput
                label="Current password"
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
                autoComplete="current-password"
              />
            )}
          </form.Field>

          <Group justify="flex-end">
            <Button variant="default" onClick={handleClose} disabled={isPending}>
              Cancel
            </Button>
            <form.Subscribe selector={(state) => state.canSubmit}>
              {(canSubmit) => (
                <Button
                  type="submit"
                  color="red"
                  loading={isPending}
                  disabled={!canSubmit}
                >
                  Delete my account
                </Button>
              )}
            </form.Subscribe>
          </Group>
        </Stack>
      </form>
    </Modal>
  );
}
