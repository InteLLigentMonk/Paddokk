import { Avatar, Group, Paper, TextInput, ActionIcon } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { SendHorizonal } from "lucide-react";
import { useAuth } from "@/hooks/use-auth";
import type { JourneyDto } from "@/generated/api/schemas";
import { JourneyCreatePostModal } from "./journey-create-post-modal";

interface JourneyCreatePostBarProps {
  journey: JourneyDto;
}

export function JourneyCreatePostBar({ journey }: JourneyCreatePostBarProps) {
  const { user } = useAuth();
  const [opened, { open, close }] = useDisclosure(false);

  return (
    <>
      <Paper withBorder p="sm" radius="md">
        <Group gap="sm" wrap="nowrap">
          <Avatar
            src={user?.image ?? null}
            radius="xl"
            size="md"
            alt={user?.name ?? "You"}
          />
          <TextInput
            placeholder="Share an update..."
            readOnly
            onClick={open}
            flex={1}
            styles={{ input: { cursor: "pointer" } }}
          />
          <ActionIcon variant="filled" size="lg" radius="xl" onClick={open} aria-label="Create post">
            <SendHorizonal size={18} />
          </ActionIcon>
        </Group>
      </Paper>

      <JourneyCreatePostModal
        journey={journey}
        opened={opened}
        onClose={close}
      />
    </>
  );
}
