import {
  ActionIcon,
  Avatar,
  Box,
  Button,
  Group,
  Paper,
  Text,
  Textarea,
} from "@mantine/core";
import { Check, Edit, X } from "lucide-react";
import { useSectionEdit } from "./use-section-edit";
import type { UserCarDto } from "@/generated/api/schemas";

interface CarOwnerNoteProps {
  car: UserCarDto;
}

export function CarOwnerNote({ car }: CarOwnerNoteProps) {
  const { isEditing, draft, setDraft, start, cancel, save, isSaving } =
    useSectionEdit<string>(car.ownerNote ?? "", Number(car.id));

  if (!car.isOwner && !car.ownerNote) return null;

  return (
    <Paper withBorder p="md">
      <Group justify="space-between" align="center" mb="sm">
        <Group gap="sm" align="center">
          <Avatar src={car.ownerAvatarUrl} size={32} radius="xl" />
          <Text
            ff="monospace"
            tt="uppercase"
            fz={10}
            fw={700}
            lts="0.12em"
            c="dimmed"
          >
            Owner's Note
          </Text>
        </Group>
        {car.isOwner && !isEditing && (
          <ActionIcon variant="subtle" size="sm" onClick={start}>
            <Edit size={14} />
          </ActionIcon>
        )}
      </Group>

      {isEditing ? (
        <Box>
          <Textarea
            value={draft}
            onChange={(e) => setDraft(e.currentTarget.value)}
            placeholder="Share a story, the car's history, or what makes it special…"
            minRows={4}
            maxRows={10}
            autosize
            maxLength={2000}
            mb="xs"
          />
          <Group gap="xs">
            <Button
              size="xs"
              onClick={() => save({ ownerNote: draft || null }, "Note saved")}
              loading={isSaving}
              leftSection={<Check size={12} />}
            >
              Save
            </Button>
            <Button
              size="xs"
              variant="subtle"
              onClick={cancel}
              disabled={isSaving}
              leftSection={<X size={12} />}
            >
              Cancel
            </Button>
          </Group>
        </Box>
      ) : (
        <Text
          fz={15}
          lh={1.7}
          fs="italic"
          c={car.ownerNote ? undefined : "dimmed"}
          style={{ whiteSpace: "pre-wrap" }}
        >
          {car.ownerNote || "No note yet."}
        </Text>
      )}
    </Paper>
  );
}
