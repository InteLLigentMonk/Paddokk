import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import type { CarSpecCategoryDto } from "@/generated/api/schemas";
import { useNotifications } from "@/integrations/mantine";
import { updateUserCarFn } from "@/lib/api/user-cars";

type CarPatch = {
  ownerNote?: string | null;
  specsByCategory?: Array<CarSpecCategoryDto> | null;
  region?: string | null;
  odometerKm?: number | null;
  drive?: number | null;
  engine?: string | null;
  nickname?: string | null;
  color?: string | null;
};

export function useSectionEdit<T>(initial: T, carId: number) {
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState<T>(initial);
  const [isSaving, setIsSaving] = useState(false);

  const start = () => {
    setDraft(initial);
    setIsEditing(true);
  };

  const cancel = () => {
    setDraft(initial);
    setIsEditing(false);
  };

  const save = async (patch: CarPatch, successMessage = "Saved") => {
    setIsSaving(true);
    try {
      await updateUserCarFn({ data: { carId, ...patch } });
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
      notifications.success({ message: successMessage });
      setIsEditing(false);
    } catch {
      notifications.error({ message: "Failed to save" });
    } finally {
      setIsSaving(false);
    }
  };

  return { isEditing, draft, setDraft, start, cancel, save, isSaving };
}
