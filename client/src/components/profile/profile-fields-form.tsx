import { useForm } from "@tanstack/react-form";
import { Button, Group, Stack, TextInput, Textarea } from "@mantine/core";
import type { UserDto } from "@/generated/api/schemas";
import { notify } from "@/integrations/mantine";
import { useUpdateCurrentUser } from "@/lib/api/users.queries";
import { updateProfileSchema } from "@/lib/validation/profile-schemas";

interface ProfileFieldsFormProps {
  user: UserDto;
}

export function ProfileFieldsForm({ user }: ProfileFieldsFormProps) {
  const updateUser = useUpdateCurrentUser();

  const form = useForm({
    defaultValues: {
      firstName: user.firstName,
      lastName: user.lastName ?? "",
      displayName: user.displayName,
      bio: user.bio ?? "",
    },
    validators: { onChange: updateProfileSchema },
    onSubmit: ({ value }) => {
      // Diagnostic backend messages are never shown (ADR-0007). Success is confirmed here;
      // failures are surfaced by the global mutation handler via the shared resolver.
      updateUser.mutate(
        {
          data: {
            firstName: value.firstName,
            lastName: value.lastName || null,
            displayName: value.displayName,
            bio: value.bio || null,
          },
        },
        {
          onSuccess: () => notify.success({ message: "Profile updated" }),
        },
      );
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
        <Group grow align="flex-start">
          <form.Field
            name="firstName"
            validators={{ onChange: updateProfileSchema.shape.firstName }}
          >
            {(field) => (
              <TextInput
                label="First name"
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
                disabled={updateUser.isPending}
                required
              />
            )}
          </form.Field>

          <form.Field
            name="lastName"
            validators={{ onChange: updateProfileSchema.shape.lastName }}
          >
            {(field) => (
              <TextInput
                label="Last name"
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
                disabled={updateUser.isPending}
              />
            )}
          </form.Field>
        </Group>

        <form.Field
          name="displayName"
          validators={{ onChange: updateProfileSchema.shape.displayName }}
        >
          {(field) => (
            <TextInput
              label="Display name"
              description="The name shown publicly on your profile and posts"
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
              disabled={updateUser.isPending}
              required
            />
          )}
        </form.Field>

        <form.Field
          name="bio"
          validators={{ onChange: updateProfileSchema.shape.bio }}
        >
          {(field) => (
            <Textarea
              label="Bio"
              description="Tell other car enthusiasts about yourself (max 500 characters)"
              autosize
              minRows={3}
              maxRows={8}
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
              disabled={updateUser.isPending}
            />
          )}
        </form.Field>

        <Group justify="flex-end">
          <form.Subscribe selector={(state) => state.canSubmit}>
            {(canSubmit) => (
              <Button
                type="submit"
                loading={updateUser.isPending}
                disabled={!canSubmit}
              >
                Save changes
              </Button>
            )}
          </form.Subscribe>
        </Group>
      </Stack>
    </form>
  );
}
