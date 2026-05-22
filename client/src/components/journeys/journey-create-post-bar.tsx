import {
  ActionIcon,
  Avatar,
  Center,
  Group,
  Paper,
  TextInput,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { SendHorizonal } from "lucide-react";
import { JourneyCreatePostModal } from "./journey-create-post-modal";
import type { JourneyDto } from "@/generated/api/schemas";
import { useAuth } from "@/hooks/use-auth";

interface JourneyCreatePostBarProps {
  journey: JourneyDto;
}

export function JourneyCreatePostBar({ journey }: JourneyCreatePostBarProps) {
  const { user } = useAuth();
  const [opened, { open, close }] = useDisclosure(false);

  return (
    <>
      <Center>
        <Paper withBorder p="sm" radius="md" w={{ base: "100%", sm: 600 }}>
          <Center>
            <Title order={4} mb="xs">
              Share an update
            </Title>
          </Center>
          <Group gap="sm" wrap="nowrap">
            <Avatar
              src={user?.image ?? null}
              radius="xl"
              size="md"
              alt="Your avatar"
            />
            <TextInput
              placeholder="Share an update..."
              readOnly
              miw={{ base: 100, sm: 400 }}
              onClick={open}
              flex={1}
              styles={{ input: { cursor: "pointer" } }}
            />
            <ActionIcon
              variant="filled"
              size="lg"
              radius="xl"
              onClick={open}
              aria-label="Create post"
            >
              <SendHorizonal size={18} />
            </ActionIcon>
          </Group>
        </Paper>
      </Center>

      <JourneyCreatePostModal
        journey={journey}
        opened={opened}
        onClose={close}
      />
    </>
  );
}
