import { AspectRatio, Box, Card, Group, SimpleGrid, Text } from "@mantine/core";
import { Images } from "lucide-react";
import { Link } from "@tanstack/react-router";
import type { FeedItemDto } from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import { OwnerLink } from "@/components/common/owner-link";
import { absoluteTime, relativeTime } from "@/lib/feed/feed-time";

interface FeedPhotosAddedCardProps {
  item: FeedItemDto;
}

/**
 * One upload session of photos to a UserCar (#187) — never one card per file. The card
 * carries the uploader, the car, the full photo count, and a small thumbnail strip that
 * deep-links into the car.
 */
export function FeedPhotosAddedCard({ item }: FeedPhotosAddedCardProps) {
  const thumbnails = item.imageUrls ?? [];
  const photoCount = item.photoCount ?? thumbnails.length;
  const remaining = photoCount - thumbnails.length;

  return (
    <Card
      withBorder
      radius="md"
      padding="sm"
      bg="light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))"
    >
      <Card.Section withBorder inheritPadding py="sm">
        <Group justify="space-between" wrap="nowrap" gap="sm">
          <OwnerLink
            target={{ kind: "user", username: item.actorUsername }}
            avatarUrl={item.actorAvatarUrl}
            primaryText={item.actorDisplayName}
            secondaryText={
              <PhotosContext item={item} photoCount={photoCount} />
            }
            avatarSize="md"
            avatarRadius="xl"
          />
          <Text
            size="xs"
            c="dimmed"
            title={absoluteTime(item.createdAt)}
            style={{ flexShrink: 0 }}
          >
            {relativeTime(item.createdAt)}
          </Text>
        </Group>
      </Card.Section>

      {thumbnails.length > 0 && item.userCarSlug != null && (
        <Card.Section>
          <Link
            to="/users/$username/cars/$slug"
            params={{ username: item.actorUsername, slug: item.userCarSlug }}
            style={{ display: "block" }}
          >
            <SimpleGrid cols={Math.min(thumbnails.length, 4)} spacing={2}>
              {thumbnails.map((url, index) => {
                const isLast = index === thumbnails.length - 1;
                return (
                  <AspectRatio key={url} ratio={1}>
                    <CdnImage src={url} width={300} alt="" fit="cover" />
                    {isLast && remaining > 0 && (
                      <Box
                        style={{
                          position: "absolute",
                          inset: 0,
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "center",
                          background: "rgba(0, 0, 0, 0.5)",
                          color: "white",
                          fontWeight: 600,
                        }}
                      >
                        +{remaining}
                      </Box>
                    )}
                  </AspectRatio>
                );
              })}
            </SimpleGrid>
          </Link>
        </Card.Section>
      )}
    </Card>
  );
}

function PhotosContext({
  item,
  photoCount,
}: {
  item: FeedItemDto;
  photoCount: number;
}) {
  const label = `added ${photoCount} ${photoCount === 1 ? "photo" : "photos"}`;
  return (
    <Group gap={4} wrap="wrap" component="span">
      <Group gap={4} wrap="nowrap" component="span">
        <Images size={13} />
        <Text size="xs" c="dimmed" component="span">
          {label}
        </Text>
      </Group>
      {item.userCarLabel && (
        <Text size="xs" fw={500} c="dimmed" component="span">
          to {item.userCarLabel}
        </Text>
      )}
    </Group>
  );
}
