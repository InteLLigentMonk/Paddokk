import {
  ActionIcon,
  Anchor,
  AspectRatio,
  Avatar,
  Card,
  Group,
  Stack,
  Text,
} from "@mantine/core";
import { Bell, Heart, Route } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import type { UserCarDto } from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";
import {
  useToggleCarLike,
  useToggleCarSubscription,
} from "@/lib/api/cars.queries";

interface CarBrowseCardProps {
  car: UserCarDto;
}

export function CarBrowseCard({ car }: CarBrowseCardProps) {
  const navigate = useNavigate();
  const carId = Number(car.id);

  const likeMutation = useToggleCarLike(carId);
  const subscribeMutation = useToggleCarSubscription(carId);

  const displayTitle = car.isCustomBuild
    ? (car.customBuildName ?? "Custom Build")
    : [car.carMakeName, car.carModelName, car.year].filter(Boolean).join(" ") ||
      "Okänd bil";

  const subtitle = car.isCustomBuild
    ? "Custom build"
    : (car.carGenerationName ?? null);

  function handleCardClick() {
    navigate({
      to: "/users/$username/cars/$slug",
      params: { username: car.ownerUsername, slug: car.slug },
    });
  }

  function handleOwnerClick(e: React.MouseEvent) {
    e.stopPropagation();
    navigate({
      to: "/users/$username",
      params: { username: car.ownerUsername },
    });
  }

  function handleLikeClick(e: React.MouseEvent) {
    e.stopPropagation();
    likeMutation.mutate(car.isLiked);
  }

  function handleSubscribeClick(e: React.MouseEvent) {
    e.stopPropagation();
    subscribeMutation.mutate(car.isSubscribed);
  }

  function handleCardKeyDown(e: React.KeyboardEvent) {
    if (e.target !== e.currentTarget) return;
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      handleCardClick();
    }
  }

  return (
    <Card
      shadow="sm"
      padding="sm"
      radius="md"
      withBorder
      role="link"
      tabIndex={0}
      aria-label={`Visa ${displayTitle}`}
      style={{ cursor: "pointer" }}
      onClick={handleCardClick}
      onKeyDown={handleCardKeyDown}
    >
      <Card.Section>
        <AspectRatio ratio={16 / 9}>
          <CdnImage
            src={car.primaryImageUrl}
            width={600}
            placeholder="car"
            alt={displayTitle}
            fit="cover"
          />
        </AspectRatio>
      </Card.Section>

      <Stack gap="xs" mt="sm" style={{ flex: 1 }}>
        <Stack gap={2}>
          <Text fw={600} size="sm" lineClamp={1}>
            {displayTitle}
          </Text>
          {subtitle && (
            <Text size="xs" c="dimmed" lineClamp={1}>
              {subtitle}
            </Text>
          )}
          {(car.nickname || car.color) && (
            <Text size="xs" c="dimmed" lineClamp={1} fs="italic">
              {[car.nickname ? `"${car.nickname}"` : null, car.color]
                .filter(Boolean)
                .join(" · ")}
            </Text>
          )}
        </Stack>

        <Group justify="space-between" align="center" wrap="nowrap" mt="auto">
          <Anchor
            component="button"
            type="button"
            onClick={handleOwnerClick}
            underline="never"
            aria-label={`Visa ${car.ownerUsername}s profil`}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 6,
              minWidth: 0,
              background: "none",
              border: "none",
              padding: 0,
              cursor: "pointer",
            }}
          >
            <Avatar
              src={optimizeImageUrl(car.ownerAvatarUrl, 80)}
              size={20}
              radius="xl"
              name={car.ownerUsername}
            />
            <Text size="xs" c="dimmed" lineClamp={1}>
              {car.ownerUsername}
            </Text>
          </Anchor>

          <Group gap={8} wrap="nowrap" style={{ flexShrink: 0 }}>
            <Group gap={3} wrap="nowrap">
              <Route size={12} color="var(--mantine-color-dimmed)" />
              <Text size="xs" c="dimmed">
                {Number(car.journeyCount)}
              </Text>
            </Group>

            <Group gap={3} wrap="nowrap">
              {car.isOwner ? (
                <Heart size={11} color="var(--mantine-color-dimmed)" />
              ) : (
                <ActionIcon
                  variant={car.isLiked ? "filled" : "subtle"}
                  color={car.isLiked ? "red" : "gray"}
                  size="xs"
                  aria-label={car.isLiked ? "Ta bort gillning" : "Gilla"}
                  loading={likeMutation.isPending}
                  onClick={handleLikeClick}
                >
                  <Heart size={11} />
                </ActionIcon>
              )}
              <Text size="xs" c="dimmed">
                {Number(car.likeCount)}
              </Text>
            </Group>

            <Group gap={3} wrap="nowrap">
              <ActionIcon
                variant={car.isSubscribed ? "filled" : "subtle"}
                color={car.isSubscribed ? "blue" : "gray"}
                size="xs"
                aria-label={car.isSubscribed ? "Avprenumerera" : "Prenumerera"}
                loading={subscribeMutation.isPending}
                onClick={handleSubscribeClick}
              >
                <Bell size={11} />
              </ActionIcon>
              <Text size="xs" c="dimmed">
                {Number(car.subscriberCount)}
              </Text>
            </Group>
          </Group>
        </Group>
      </Stack>
    </Card>
  );
}
