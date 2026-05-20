import { useState } from "react";
import { Box, ActionIcon } from "@mantine/core";
import { Edit } from "lucide-react";
import { useRouter } from "@tanstack/react-router";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto, CarImageDto } from "@/generated/api/schemas";
import { CarImageCarousel } from "@/components/cars/car-image-carousel";
import { EditCarImagesSection } from "@/components/cars/edit-car-images-section";
import {
  deleteCarImageFn,
  updateCarImageFn,
} from "@/lib/api/car-images.server";
import {
  carImagesUploadCarImage,
  carImagesSetPrimaryImage,
} from "@/generated/api/car-images/car-images";
import type { EditCarImagesSectionProps } from "@/components/cars/edit-car-images-section";
import type { PendingImage } from "@/components/cars/car-form-stepper";
import { CarSectionHead } from "./car-section-head";

type PrimaryId = EditCarImagesSectionProps["primaryId"];

interface CarPhotosSectionProps {
  car: UserCarDto;
  images: CarImageDto[];
}

export function CarPhotosSection({ car, images }: CarPhotosSectionProps) {
  const router = useRouter();
  const notifications = useNotifications();
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const [existingImages, setExistingImages] = useState<CarImageDto[]>(images);
  const [pendingImages, setPendingImages] = useState<PendingImage[]>([]);
  const [deletedImageIds, setDeletedImageIds] = useState<number[]>([]);
  const [reorderedIds, setReorderedIds] = useState<number[] | null>(null);

  const initialPrimaryId: PrimaryId = (() => {
    const primary = images.find((img) => img.isPrimary);
    return primary ? { type: "existing", id: Number(primary.id) } : null;
  })();
  const [primaryId, setPrimaryId] = useState<PrimaryId>(initialPrimaryId);

  const displayName =
    car.nickname ||
    (car.isCustomBuild
      ? (car.customBuildName ?? "Custom Build")
      : [car.carMakeName, car.carModelName].filter(Boolean).join(" "));

  const handleSave = async () => {
    setIsSaving(true);
    try {
      for (const id of deletedImageIds) {
        await deleteCarImageFn({ data: { carId: Number(car.id), imageId: id } });
      }
      for (const img of pendingImages) {
        await carImagesUploadCarImage(Number(car.id), { File: img.file });
      }
      if (reorderedIds) {
        for (let i = 0; i < reorderedIds.length; i++) {
          await updateCarImageFn({
            data: { carId: Number(car.id), imageId: reorderedIds[i], sortOrder: i },
          });
        }
      }
      if (primaryId?.type === "existing") {
        const originalPrimary = images.find((img) => img.isPrimary);
        if (!originalPrimary || Number(originalPrimary.id) !== primaryId.id) {
          await carImagesSetPrimaryImage(Number(car.id), primaryId.id);
        }
      }
      await router.invalidate();
      notifications.success({ message: "Photos updated" });
      setIsEditing(false);
    } catch {
      notifications.error({ message: "Failed to save photos" });
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setExistingImages(images);
    setPendingImages([]);
    setDeletedImageIds([]);
    setReorderedIds(null);
    setPrimaryId(initialPrimaryId);
    setIsEditing(false);
  };

  return (
    <Box>
      <Box style={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between" }} mb="md">
        <CarSectionHead kicker="In the metal" title="Photos" count={images.length} mb={0} />
        {car.isOwner && !isEditing && (
          <ActionIcon variant="subtle" size="sm" mt={4} onClick={() => setIsEditing(true)}>
            <Edit size={14} />
          </ActionIcon>
        )}
      </Box>

      {isEditing ? (
        <EditCarImagesSection
          existingImages={existingImages}
          pendingImages={pendingImages}
          deletedImageIds={deletedImageIds}
          primaryId={primaryId}
          onPendingChange={setPendingImages}
          onDeleteExisting={(id) => {
            setDeletedImageIds((prev) => [...prev, id]);
            if (primaryId?.type === "existing" && primaryId.id === id) {
              setPrimaryId(null);
            }
          }}
          onReorderExisting={(ids) => {
            setReorderedIds(ids);
            setExistingImages((prev) => {
              const byId = Object.fromEntries(prev.map((img) => [Number(img.id), img]));
              return ids.map((id) => byId[id]).filter(Boolean);
            });
          }}
          onSetPrimary={setPrimaryId}
          isSubmitting={isSaving}
        />
      ) : (
        <CarImageCarousel images={images} displayName={displayName} />
      )}

      {isEditing && (
        <Box mt="sm" style={{ display: "flex", gap: 8 }}>
          <button
            onClick={handleSave}
            disabled={isSaving}
            style={{
              padding: "6px 16px",
              background: "var(--mantine-primary-color-filled)",
              color: "white",
              border: "none",
              borderRadius: "var(--mantine-radius-sm)",
              cursor: isSaving ? "not-allowed" : "pointer",
              fontSize: 13,
            }}
          >
            {isSaving ? "Saving..." : "Save photos"}
          </button>
          <button
            onClick={handleCancel}
            disabled={isSaving}
            style={{
              padding: "6px 16px",
              background: "transparent",
              color: "inherit",
              border: "1px solid var(--mantine-color-default-border)",
              borderRadius: "var(--mantine-radius-sm)",
              cursor: isSaving ? "not-allowed" : "pointer",
              fontSize: 13,
            }}
          >
            Cancel
          </button>
        </Box>
      )}
    </Box>
  );
}
