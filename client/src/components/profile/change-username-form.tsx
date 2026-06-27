import { useForm } from "@tanstack/react-form";
import { Alert, Button, Group, Stack, Text, TextInput } from "@mantine/core";
import { Info } from "lucide-react";
import type { UserDto } from "@/generated/api/schemas";
import { notify } from "@/integrations/mantine";
import { SEVERITY_COLOR, resolveApiError } from "@/lib/api/error-resolver";
import { useChangeUsername } from "@/lib/api/users.queries";
import { changeUsernameSchema } from "@/lib/validation/profile-schemas";

interface ChangeUsernameFormProps {
  user: UserDto;
}

export function ChangeUsernameForm({ user }: ChangeUsernameFormProps) {
  const changeUsername = useChangeUsername();

  const form = useForm({
    defaultValues: { username: user.username },
    validators: { onChange: changeUsernameSchema },
    onSubmit: ({ value }) => {
      if (value.username === user.username) return;
      changeUsername.mutate(
        { data: { username: value.username } },
        {
          onSuccess: () => notify.success({ message: "Username updated" }),
        },
      );
    },
  });

  // Diagnostic backend messages are never shown (ADR-0007); inline copy comes from
  // the resolver, keyed on the error code (USERNAME_TAKEN, USERNAME_CHANGE_TOO_SOON, ...).
  const resolvedError = changeUsername.isError
    ? resolveApiError(changeUsername.error)
    : null;

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
                field.state.meta.isTouched && field.state.meta.errors.length > 0
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

        {resolvedError && (
          <Alert color={SEVERITY_COLOR[resolvedError.severity]} variant="light">
            {resolvedError.message}
          </Alert>
        )}

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
