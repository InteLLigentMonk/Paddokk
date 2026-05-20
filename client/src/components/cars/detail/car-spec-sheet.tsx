import {
  Box,
  Button,
  Divider,
  Group,
  ActionIcon,
  Stack,
  Text,
  TextInput,
  Paper,
  Flex,
} from "@mantine/core";
import { Edit, Check, X, Plus, Trash2, DotIcon } from "lucide-react";
import type { UserCarDto, CarSpecCategoryDto } from "@/generated/api/schemas";
import { CarSectionHead } from "./car-section-head";
import { useSectionEdit } from "./use-section-edit";

interface CarSpecSheetProps {
  car: UserCarDto;
}

function CategoryRow({
  category,
  items,
  isFirst,
}: CarSpecCategoryDto & { isFirst?: boolean }) {
  return (
    <Group
      gap={0}
      align="flex-start"
      wrap="nowrap"
      style={
        isFirst
          ? undefined
          : {
              borderTop:
                "1px solid light-dark(var(--mantine-color-gray-2), var(--mantine-color-dark-5))",
            }
      }
      py={10}
    >
      <Box style={{ minWidth: 180, flexShrink: 0 }} pr="md">
        <Text
          ff="monospace"
          tt="uppercase"
          fz={10}
          fw={700}
          c="dimmed"
          lts="0.1em"
        >
          {category}
        </Text>
      </Box>
      <Stack gap={2} style={{ flex: 1 }}>
        {items.map((item, i) => (
          <Flex key={i} align="center">
            <DotIcon size={24} color="var(--mantine-primary-color-5)" />
            <Text fz={13} lh={1.5}>
              {item}
            </Text>
          </Flex>
        ))}
      </Stack>
    </Group>
  );
}

interface EditableCategoryGroupProps {
  entry: CarSpecCategoryDto;
  index: number;
  onChange: (index: number, updated: CarSpecCategoryDto) => void;
  onRemove: (index: number) => void;
}

function EditableCategoryGroup({
  entry,
  index,
  onChange,
  onRemove,
}: EditableCategoryGroupProps) {
  const updateItem = (itemIndex: number, value: string) => {
    const items = [...entry.items];
    items[itemIndex] = value;
    onChange(index, { ...entry, items });
  };

  const addItem = () => {
    onChange(index, { ...entry, items: [...entry.items, ""] });
  };

  const removeItem = (itemIndex: number) => {
    onChange(index, {
      ...entry,
      items: entry.items.filter((_, i) => i !== itemIndex),
    });
  };

  return (
    <Box
      p="sm"
      style={{
        border:
          "1px solid light-dark(var(--mantine-color-gray-3), var(--mantine-color-dark-4))",
        borderRadius: "var(--mantine-radius-sm)",
      }}
    >
      <Group justify="space-between" mb="xs">
        <TextInput
          placeholder="Category (e.g. Engine)"
          value={entry.category}
          onChange={(e) =>
            onChange(index, { ...entry, category: e.currentTarget.value })
          }
          size="xs"
          style={{ flex: 1 }}
        />
        <ActionIcon
          variant="subtle"
          color="red"
          size="sm"
          onClick={() => onRemove(index)}
        >
          <Trash2 size={14} />
        </ActionIcon>
      </Group>

      <Stack gap={4} pl="lg">
        {entry.items.map((item, itemIndex) => (
          <Group key={itemIndex} gap="xs" wrap="nowrap">
            <TextInput
              placeholder="Spec item"
              value={item}
              onChange={(e) => updateItem(itemIndex, e.currentTarget.value)}
              size="xs"
              style={{ flex: 1 }}
            />
            <ActionIcon
              variant="subtle"
              color="red"
              size="xs"
              onClick={() => removeItem(itemIndex)}
            >
              <X size={12} />
            </ActionIcon>
          </Group>
        ))}
        <Button
          size="xs"
          variant="subtle"
          leftSection={<Plus size={12} />}
          onClick={addItem}
        >
          Add item
        </Button>
      </Stack>
    </Box>
  );
}

export function CarSpecSheet({ car }: CarSpecSheetProps) {
  const specs = car.specsByCategory ?? [];
  const { isEditing, draft, setDraft, start, cancel, save, isSaving } =
    useSectionEdit<CarSpecCategoryDto[]>(specs, Number(car.id));

  const addCategory = () => {
    setDraft([...draft, { category: "", items: [""] }]);
  };

  const updateCategory = (index: number, updated: CarSpecCategoryDto) => {
    setDraft(draft.map((c, i) => (i === index ? updated : c)));
  };

  const removeCategory = (index: number) => {
    setDraft(draft.filter((_, i) => i !== index));
  };

  const handleSave = () => {
    const cleaned = draft
      .filter((c) => c.category.trim())
      .map((c) => ({ ...c, items: c.items.filter((item) => item.trim()) }));
    save({ specsByCategory: cleaned }, "Spec sheet saved");
  };

  if (!car.isOwner && specs.length === 0) return null;

  return (
    <Box>
      <CarSectionHead
        kicker="Spec sheet"
        title="Build & mods"
        count={specs.length > 0 ? specs.length : undefined}
        rightAction={
          car.isOwner && !isEditing ? (
            <ActionIcon variant="subtle" size="sm" onClick={start}>
              <Edit size={14} />
            </ActionIcon>
          ) : undefined
        }
      />

      <Paper withBorder radius="md" p="sm">
        {isEditing ? (
          <Stack gap="sm">
            {draft.map((entry, index) => (
              <EditableCategoryGroup
                key={index}
                entry={entry}
                index={index}
                onChange={updateCategory}
                onRemove={removeCategory}
              />
            ))}
            <Button
              size="xs"
              variant="light"
              leftSection={<Plus size={12} />}
              onClick={addCategory}
            >
              Add category
            </Button>
            <Divider />
            <Group gap="xs">
              <Button
                size="xs"
                onClick={handleSave}
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
          </Stack>
        ) : specs.length > 0 ? (
          <Box>
            {specs.map((entry, i) => (
              <CategoryRow key={i} {...entry} isFirst={i === 0} />
            ))}
          </Box>
        ) : (
          <Text fz={13} c="dimmed" py="sm">
            No specs added yet.
          </Text>
        )}
      </Paper>
    </Box>
  );
}
