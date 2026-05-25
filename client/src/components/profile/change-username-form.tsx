import { useForm } from "@tanstack/react-form";
import { Alert, Button, Group, Stack, Text, TextInput } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { Info } from "lucide-react";
import { ApiError } from "@/lib/api/api-error";
import { useChangeUsername } from "@/lib/api/users.queries";
import { changeUsernameSchema } from "@/lib/validation/profile-schemas";
import type { UserDto } from "@/generated/api/schemas";

interface ChangeUsernameFormProps {
  user: UserDto;
}

export function ChangeUsernameForm({ user }: ChangeUsernameFormProps) {
  const changeUsername = useChangeUsername();

  const form = useForm({
    defaultValues: { username: user.username },
    validators: { onChange: changeUsernameSchema },
    onSubmit: async ({ value }) => {
      if (value.username === user.username) return;
      try {
        await changeUsername.mutateAsync({ data: { username: value.username } });
        notifications.show({
          color: "green",
          message: "Username updated",
        });
      } catch (error) {
        notifications.show({
          color: "red",
          message:
            error instanceof ApiError
              ? error.message
              : "Could not change username. Please try again.",
        });
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
        <Alert icon={<Info size={16} />} color="blue" variant="light">
          <Text size="sm">
            Your username appears in your profile URL. You can change it once
            every 30 days.
          </Text>
        </Alert>

        <form.Field
          name="username"
          validators={{ onChange: changeUsernameSchema.shape.username }}
        >
          {(field) => (
            <TextInput
              label="Username"
              description="Lowercase letters, digits, dot, underscore or hyphen"
              value={field.state.value}
              onChange={(e) =>
                field.handleChange(e.target.value.toLowerCase().trim())
              }
              onBlur={field.handleBlur}
              error={
                field.state.meta.isTouched &&
                field.state.meta.errors.length > 0
                  ? field.state.meta.errors
                      .map((e) => e?.message ?? "")
                      .join(", ")
                  : undefined
              }
              disabled={changeUsername.isPending}
              required
            />
          )}
        </form.Field>

        <Group justify="flex-end">
          <form.Subscribe
            selector={(state) => ({
              canSubmit: state.canSubmit,
              username: state.values.username,
            })}
          >
            {({ canSubmit, username }) => (
              <Button
                type="submit"
                loading={changeUsername.isPending}
                disabled={!canSubmit || username === user.username}
              >
                Change username
              </Button>
            )}
          </form.Subscribe>
        </Group>
      </Stack>
    </form>
  );
}
