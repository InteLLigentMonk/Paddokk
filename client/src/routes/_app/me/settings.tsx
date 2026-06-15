import { useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import {
  Button,
  Card,
  Container,
  Divider,
  Group,
  SimpleGrid,
  Stack,
  Tabs,
  Text,
  Title,
} from "@mantine/core";
import { Cookie, Database, Lock, Mail, Trash2, User } from "lucide-react";
import { currentUserQueryOptions } from "@/lib/api/users.queries";
import { ManageCookies } from "@/components/consent";
import { ChangeEmailForm } from "@/components/profile/change-email-form";
import { ChangePasswordForm } from "@/components/profile/change-password-form";
import { ChangeUsernameForm } from "@/components/profile/change-username-form";
import { DeleteAccountModal } from "@/components/profile/delete-account-modal";
import { ExportDataAction } from "@/components/profile/export-data-action";
import { ProfileFieldsForm } from "@/components/profile/profile-fields-form";

export const Route = createFileRoute("/_app/me/settings")({
  staticData: { breadcrumb: "Settings" },
  loader: ({ context: { queryClient } }) =>
    queryClient.ensureQueryData(currentUserQueryOptions()),
  component: SettingsPage,
});

function SettingsPage() {
  const { data: user } = useSuspenseQuery(currentUserQueryOptions());
  const [deleteModalOpened, setDeleteModalOpened] = useState(false);

  return (
    <Container size="md" py="xl">
      <Stack gap="xl">
        <Stack gap={4}>
          <Title order={1}>Settings</Title>
          <Text c="dimmed">Manage your profile, account and security</Text>
        </Stack>

        <Tabs defaultValue="profile" keepMounted={false}>
          <Tabs.List>
            <Tabs.Tab value="profile" leftSection={<User size={16} />}>
              Profile
            </Tabs.Tab>
            <Tabs.Tab value="account" leftSection={<Mail size={16} />}>
              Account
            </Tabs.Tab>
            <Tabs.Tab value="security" leftSection={<Lock size={16} />}>
              Security
            </Tabs.Tab>
            <Tabs.Tab value="privacy" leftSection={<Cookie size={16} />}>
              Privacy
            </Tabs.Tab>
            <Tabs.Tab value="data" leftSection={<Database size={16} />}>
              Your data
            </Tabs.Tab>
          </Tabs.List>

          <Tabs.Panel value="profile" pt="lg">
            <Stack gap="lg">
              <Card withBorder radius="md" padding="lg">
                <Stack gap="md">
                  <Stack gap={2}>
                    <Title order={3}>Profile details</Title>
                    <Text size="sm" c="dimmed">
                      Update your public profile information
                    </Text>
                  </Stack>
                  <Divider />
                  <ProfileFieldsForm user={user} />
                </Stack>
              </Card>

              <Card withBorder radius="md" padding="lg">
                <Stack gap="md">
                  <Stack gap={2}>
                    <Title order={3}>Username</Title>
                    <Text size="sm" c="dimmed">
                      Change the @handle shown in your profile URL
                    </Text>
                  </Stack>
                  <Divider />
                  <ChangeUsernameForm user={user} />
                </Stack>
              </Card>
            </Stack>
          </Tabs.Panel>

          <Tabs.Panel value="account" pt="lg">
            <Card withBorder radius="md" padding="lg">
              <Stack gap="md">
                <Stack gap={2}>
                  <Title order={3}>Email address</Title>
                  <Text size="sm" c="dimmed">
                    Change the email address used for sign-in and notifications
                  </Text>
                </Stack>
                <Divider />
                <ChangeEmailForm user={user} />
              </Stack>
            </Card>
          </Tabs.Panel>

          <Tabs.Panel value="security" pt="lg">
            <Card withBorder radius="md" padding="lg">
              <Stack gap="md">
                <Stack gap={2}>
                  <Title order={3}>Change password</Title>
                  <Text size="sm" c="dimmed">
                    Update the password used to sign in
                  </Text>
                </Stack>
                <Divider />
                <ChangePasswordForm />
              </Stack>
            </Card>
          </Tabs.Panel>

          <Tabs.Panel value="privacy" pt="lg">
            <Card withBorder radius="md" padding="lg">
              <Stack gap="md">
                <Stack gap={2}>
                  <Title order={3}>Cookies</Title>
                  <Text size="sm" c="dimmed">
                    Review or change your cookie choices
                  </Text>
                </Stack>
                <Divider />
                <Group justify="space-between" wrap="nowrap">
                  <Text size="sm" c="dimmed">
                    Reopen the cookie consent banner to update which
                    non-essential cookies you allow. Essential cookies are
                    always active.
                  </Text>
                  <ManageCookies variant="button" />
                </Group>
              </Stack>
            </Card>
          </Tabs.Panel>

          <Tabs.Panel value="data" pt="lg">
            <Card withBorder radius="md" padding="lg">
              <Stack gap="md">
                <Stack gap={2}>
                  <Title order={3}>Your data</Title>
                  <Text size="sm" c="dimmed">
                    Exercise your GDPR rights over the data Paddokk holds about
                    you
                  </Text>
                </Stack>
                <Divider />
                <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="xl">
                  <Stack gap="xs" align="flex-start">
                    <Title order={4}>Export my data</Title>
                    <Text size="sm" c="dimmed">
                      Right of access — download everything Paddokk holds about
                      you
                    </Text>
                    <ExportDataAction />
                  </Stack>

                  <Stack gap="xs" align="flex-start">
                    <Title order={4} c="red">
                      Delete my account
                    </Title>
                    <Text size="sm" c="dimmed">
                      Right to erasure — permanently delete your account and all
                      your data
                    </Text>
                    <Button
                      color="red"
                      variant="light"
                      leftSection={<Trash2 size={16} />}
                      onClick={() => setDeleteModalOpened(true)}
                    >
                      Delete account
                    </Button>
                  </Stack>
                </SimpleGrid>
              </Stack>
            </Card>
          </Tabs.Panel>
        </Tabs>
      </Stack>

      <DeleteAccountModal
        opened={deleteModalOpened}
        onClose={() => setDeleteModalOpened(false)}
      />
    </Container>
  );
}
