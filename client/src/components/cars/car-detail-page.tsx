import { useState, useEffect } from "react";
import {
  Container,
  Title,
  Text,
  Group,
  Stack,
  Button,
  TextInput,
  Badge,
  Anchor,
  Divider,
  Paper,
  SimpleGrid,
  Typography,
} from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import { Image } from "@mantine/core";
import { Link, useRouter } from "@tanstack/react-router";
import { ArrowLeft, Edit, X, Check } from "lucide-react";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto, CarImageDto } from "@/generated/api/schemas";
import { updateUserCarFn } from "@/lib/api/user-cars.server";
import {
  deleteCarImageFn,
  updateCarImageFn,
} from "@/lib/api/car-images.server";
import {
  carImagesUploadCarImage,
  carImagesSetPrimaryImage,
} from "@/generated/api/car-images/car-images";
import type { EditCarImagesSectionProps } from "./edit-car-images-section";
import { EditCarImagesSection } from "./edit-car-images-section";
import { CarSpecsEditor } from "./car-specs-editor";
import type { PendingImage } from "./car-form-stepper";

type PrimaryId = EditCarImagesSectionProps["primaryId"];

interface CarDetailPageProps {
  car: UserCarDto;
  images: CarImageDto[];
  startInEditMode?: boolean;
}

export function CarDetailPage({
  car,
  images,
  startInEditMode,
}: CarDetailPageProps) {
  const carAny = car as typeof car & { isCustomBuild?: boolean; customBuildName?: string | null };

  const [isEditing, setIsEditing] = useState(startInEditMode ?? false);
  const [isSaving, setIsSaving] = useState(false);

  const [nickname, setNickname] = useState(car.nickname ?? "");
  const [color, setColor] = useState(car.color ?? "");
  const [specs, setSpecs] = useState(car.description ?? "");
  const [customBuildName, setCustomBuildName] = useState(carAny.customBuildName ?? "");

  const [existingImages, setExistingImages] = useState<CarImageDto[]>(images);
  const [pendingImages, setPendingImages] = useState<PendingImage[]>([]);
  const [deletedImageIds, setDeletedImageIds] = useState<number[]>([]);
  const [reorderedIds, setReorderedIds] = useState<number[] | null>(null);

  const initialPrimaryId: PrimaryId = (() => {
    const primary = images.find((img) => img.isPrimary);
    return primary ? { type: "existing", id: Number(primary.id) } : null;
  })();
  const [primaryId, setPrimaryId] = useState<PrimaryId>(initialPrimaryId);

  const router = useRouter();
  const notifications = useNotifications();

  useEffect(() => {
    if (!isEditing) {
      setExistingImages(images);
      setPendingImages([]);
      setDeletedImageIds([]);
      setReorderedIds(null);
      const primary = images.find((img) => img.isPrimary);
      setPrimaryId(
        primary ? { type: "existing", id: Number(primary.id) } : null,
      );
    }
  }, [images]); // eslint-disable-line react-hooks/exhaustive-deps

  const carId = Number(car.id);
  const displayName =
    car.nickname ||
    (carAny.isCustomBuild
      ? (carAny.customBuildName ?? "Custom Build")
      : `${car.carMakeName} ${car.carModelName}`);

  const handleCancelEdit = () => {
    setNickname(car.nickname ?? "");
    setColor(car.color ?? "");
    setSpecs(car.description ?? "");
    setCustomBuildName(carAny.customBuildName ?? "");
    setExistingImages(images);
    setPendingImages([]);
    setDeletedImageIds([]);
    setReorderedIds(null);
    setPrimaryId(initialPrimaryId);
    setIsEditing(false);
  };

  const handleSave = async () => {
    setIsSaving(true);
    try {
      await updateUserCarFn({
        data: {
          carId,
          customBuildName: carAny.isCustomBuild ? customBuildName : undefined,
          nickname,
          color,
          description: specs,
          isPrimary: car.isPrimary,
        },
      });

      for (const id of deletedImageIds) {
        await deleteCarImageFn({ data: { carId, imageId: id } });
      }

      for (const img of pendingImages) {
        await carImagesUploadCarImage(carId, { File: img.file });
      }

      if (reorderedIds) {
        for (let i = 0; i < reorderedIds.length; i++) {
          await updateCarImageFn({
            data: { carId, imageId: reorderedIds[i], sortOrder: i },
          });
        }
      }

      if (primaryId?.type === "existing") {
        const originalPrimary = images.find((img) => img.isPrimary);
        if (!originalPrimary || Number(originalPrimary.id) !== primaryId.id) {
          await carImagesSetPrimaryImage(carId, primaryId.id);
        }
      }

      await router.invalidate();
      notifications.success({ message: "Car updated successfully" });
      setIsEditing(false);
    } catch {
      notifications.error({ message: "Failed to save changes" });
    } finally {
      setIsSaving(false);
    }
  };

  const handleDeleteExisting = (id: number) => {
    setDeletedImageIds((prev) => [...prev, id]);
    if (primaryId?.type === "existing" && primaryId.id === id) {
      setPrimaryId(null);
    }
  };

  const handleReorderExisting = (ids: number[]) => {
    setReorderedIds(ids);
    setExistingImages((prev) => {
      const byId = Object.fromEntries(prev.map((img) => [Number(img.id), img]));
      return ids.map((id) => byId[id]).filter(Boolean);
    });
  };

  return (
    <Container size="md" py="xl">
      <Group mb="lg">
        <Anchor component={Link} to="/cars" c="dimmed" size="sm">
          <Group gap={4}>
            <ArrowLeft size={14} />
            <span>Back to garage</span>
          </Group>
        </Anchor>
      </Group>

      <Group justify="space-between" mb="lg" align="flex-start">
        <Stack gap={4}>
          <Title order={1}>{displayName}</Title>
          <Text c="dimmed">
            {carAny.isCustomBuild
              ? "Custom Build"
              : [car.carMakeName, car.carModelName, car.carGenerationName, String(car.year ?? "")]
                  .filter(Boolean)
                  .join(" · ")}
          </Text>
        </Stack>

        {!isEditing ? (
          <Button
            leftSection={<Edit size={16} />}
            variant="subtle"
            onClick={() => setIsEditing(true)}
          >
            Edit
          </Button>
        ) : (
          <Group>
            <Button
              variant="subtle"
              leftSection={<X size={16} />}
              onClick={handleCancelEdit}
              disabled={isSaving}
            >
              Cancel
            </Button>
            <Button
              leftSection={<Check size={16} />}
              onClick={handleSave}
              loading={isSaving}
            >
              Save
            </Button>
          </Group>
        )}
      </Group>

      {isEditing ? (
        <EditCarImagesSection
          existingImages={existingImages}
          pendingImages={pendingImages}
          deletedImageIds={deletedImageIds}
          primaryId={primaryId}
          onPendingChange={setPendingImages}
          onDeleteExisting={handleDeleteExisting}
          onReorderExisting={handleReorderExisting}
          onSetPrimary={setPrimaryId}
          isSubmitting={isSaving}
        />
      ) : (
        <CarImageCarousel images={images} displayName={displayName} />
      )}

      <Divider my="xl" />

      <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="md" mb="xl">
        {carAny.isCustomBuild ? (
          <Paper withBorder p="md" radius="md">
            <Text size="xs" c="dimmed" fw={500} mb={4}>
              Build name
            </Text>
            {isEditing ? (
              <TextInput
                value={customBuildName}
                onChange={(e) => setCustomBuildName(e.currentTarget.value)}
                placeholder="e.g. SR20DET S13 kouki"
                size="sm"
                variant="unstyled"
              />
            ) : (
              <Text>{carAny.customBuildName || "Custom Build"}</Text>
            )}
          </Paper>
        ) : (
          <>
            <Paper withBorder p="md" radius="md">
              <Text size="xs" c="dimmed" fw={500} mb={4}>
                Make
              </Text>
              <Text>{car.carMakeName}</Text>
            </Paper>
            <Paper withBorder p="md" radius="md">
              <Text size="xs" c="dimmed" fw={500} mb={4}>
                Model
              </Text>
              <Text>{car.carModelName}</Text>
            </Paper>
            {car.carGenerationName && (
              <Paper withBorder p="md" radius="md">
                <Text size="xs" c="dimmed" fw={500} mb={4}>
                  Generation
                </Text>
                <Text>{car.carGenerationName}</Text>
              </Paper>
            )}
            <Paper withBorder p="md" radius="md">
              <Text size="xs" c="dimmed" fw={500} mb={4}>
                Year
              </Text>
              <Text>{String(car.year)}</Text>
            </Paper>
          </>
        )}
        <Paper withBorder p="md" radius="md">
          <Text size="xs" c="dimmed" fw={500} mb={4}>
            Nickname
          </Text>
          {isEditing ? (
            <TextInput
              value={nickname}
              onChange={(e) => setNickname(e.currentTarget.value)}
              placeholder="e.g. Drift Missile"
              size="sm"
              variant="unstyled"
            />
          ) : (
            <Text c={car.nickname ? undefined : "dimmed"}>
              {car.nickname || "None"}
            </Text>
          )}
        </Paper>
        <Paper withBorder p="md" radius="md">
          <Text size="xs" c="dimmed" fw={500} mb={4}>
            Color
          </Text>
          {isEditing ? (
            <TextInput
              value={color}
              onChange={(e) => setColor(e.currentTarget.value)}
              placeholder="e.g. Midnight Blue"
              size="sm"
              variant="unstyled"
            />
          ) : (
            <Text c={car.color ? undefined : "dimmed"}>
              {car.color || "Unknown"}
            </Text>
          )}
        </Paper>
        {Number(car.journeyCount) > 0 && (
          <Paper withBorder p="md" radius="md">
            <Text size="xs" c="dimmed" fw={500} mb={4}>
              Journeys
            </Text>
            <Badge variant="light">{car.journeyCount}</Badge>
          </Paper>
        )}
      </SimpleGrid>

      <Stack gap="sm">
        <Text fw={500}>Specs</Text>
        {isEditing ? (
          <CarSpecsEditor content={specs} onChange={setSpecs} />
        ) : car.description ? (
          <Paper withBorder p="md" radius="md">
            <Typography>
              <div dangerouslySetInnerHTML={{ __html: car.description }} />
            </Typography>
          </Paper>
        ) : (
          <Text c="dimmed" size="sm">
            No specs added yet.
          </Text>
        )}
      </Stack>
    </Container>
  );
}

interface CarImageCarouselProps {
  images: CarImageDto[];
  displayName: string;
}

function CarImageCarousel({ images, displayName }: CarImageCarouselProps) {
  if (images.length === 0) {
    return (
      <Image
        src="https://placehold.co/800x450/e9ecef/495057?text=No+Photos"
        alt={displayName}
        radius="md"
        h={380}
        fit="cover"
        mb="xl"
      />
    );
  }

  if (images.length === 1) {
    return (
      <Image
        src={images[0].imageUrl}
        alt={images[0].caption ?? displayName}
        radius="md"
        h={380}
        fit="cover"
        mb="xl"
      />
    );
  }

  return (
    <Carousel withIndicators emblaOptions={{ loop: true }} mb="xl" height={380}>
      {images.map((img) => (
        <Carousel.Slide key={String(img.id)}>
          <Image
            src={img.imageUrl}
            alt={img.caption ?? displayName}
            h={380}
            fit="cover"
            radius="md"
          />
        </Carousel.Slide>
      ))}
    </Carousel>
  );
}
