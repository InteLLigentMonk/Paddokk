import { useEffect, useRef, useState } from "react";
import { Anchor, AspectRatio, Card, Group, Stack, Text } from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import { ChevronDown, ChevronUp, MessageSquare } from "lucide-react";
import { Link } from "@tanstack/react-router";
import type { FeedItemDto } from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";
import { OwnerLink } from "@/components/common/owner-link";
import { absoluteTime, relativeTime } from "@/lib/feed/feed-time";

interface FeedJourneyPostCardProps {
  item: FeedItemDto;
}

/**
 * A JourneyPost as it appears in the personalised feed. Unlike the journey-detail
 * post card, this one carries its own context — which Journey and UserCar the post
 * belongs to — because the feed mixes posts from many sources, and deep-links back
 * to that source Journey (ADR-0006).
 */
export function FeedJourneyPostCard({ item }: FeedJourneyPostCardProps) {
  const images = item.imageUrls ?? [];
  const commentCount = item.commentCount ?? 0;

  // Only the journey owner can author posts today, so the actor doubles as the
  // owner whose username keys the journey route. When collaborator posting lands,
  // the projection gains an explicit owner username and this uses that instead.
  const journeySlug = item.journeySlug;

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
            secondaryText={<FeedPostContext item={item} />}
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

      {images.length > 0 && (
        <Card.Section>
          <FeedPostImages
            images={images}
            alt={item.journeyTitle ?? "Journey post"}
          />
        </Card.Section>
      )}

      {item.textContent && (
        <Card.Section
          withBorder
          inheritPadding
          py="sm"
          bg="light-dark(var(--mantine-color-white), var(--mantine-color-dark-8))"
        >
          <FeedPostText text={item.textContent} />
        </Card.Section>
      )}

      <Card.Section withBorder inheritPadding py="xs">
        <Group justify="space-between" wrap="nowrap">
          <Group gap={4} wrap="nowrap" c="dimmed">
            <MessageSquare size={14} />
            <Text size="sm">{commentCount}</Text>
          </Group>
          {journeySlug != null && (
            <Link
              to="/users/$username/journeys/$slug"
              params={{ username: item.actorUsername, slug: journeySlug }}
              style={{ textDecoration: "none" }}
            >
              <Text
                size="sm"
                fw={500}
                style={{ color: "var(--mantine-color-anchor)" }}
              >
                View journey
              </Text>
            </Link>
          )}
        </Group>
      </Card.Section>
    </Card>
  );
}

/** "posted in [Journey] · [Car]" — the source context the feed adds over a bare post. */
function FeedPostContext({ item }: { item: FeedItemDto }) {
  return (
    <Group gap={4} wrap="wrap" component="span">
      <Text size="xs" c="dimmed" component="span">
        posted in
      </Text>
      {item.journeyTitle && (
        <Text size="xs" fw={500} c="dimmed" component="span">
          {item.journeyTitle}
        </Text>
      )}
      {item.userCarLabel && (
        <Text size="xs" c="dimmed" component="span">
          · {item.userCarLabel}
        </Text>
      )}
    </Group>
  );
}

function FeedPostImages({
  images,
  alt,
}: {
  images: Array<string>;
  alt: string;
}) {
  if (images.length === 1) {
    return (
      <AspectRatio ratio={16 / 9}>
        <CdnImage src={images[0]} width={1200} alt={alt} fit="cover" />
      </AspectRatio>
    );
  }

  return (
    <AspectRatio ratio={16 / 9}>
      <Carousel height="100%" slideSize="100%" slideGap="xs">
        {images.map((url, i) => (
          <Carousel.Slide key={url}>
            <CdnImage
              src={url}
              width={1200}
              alt={`${alt} ${i + 1}`}
              fit="cover"
              h="100%"
            />
          </Carousel.Slide>
        ))}
      </Carousel>
    </AspectRatio>
  );
}

function FeedPostText({ text }: { text: string }) {
  const [expanded, setExpanded] = useState(false);
  const [overflows, setOverflows] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (ref.current) {
      setOverflows(ref.current.scrollHeight > ref.current.clientHeight + 1);
    }
  }, [text]);

  return (
    <Stack gap="xs">
      <div
        ref={ref}
        dangerouslySetInnerHTML={{ __html: text }}
        style={{
          wordBreak: "break-word",
          ...(!expanded && {
            display: "-webkit-box",
            WebkitLineClamp: 6,
            WebkitBoxOrient: "vertical",
            overflow: "hidden",
          }),
        }}
      />
      {(overflows || expanded) && (
        <Anchor
          component="button"
          size="xs"
          onClick={() => setExpanded((v) => !v)}
        >
          <Group gap={4} align="center" wrap="nowrap">
            {expanded ? <ChevronUp size={14} /> : <ChevronDown size={14} />}
            {expanded ? "Show less" : "Show more"}
          </Group>
        </Anchor>
      )}
    </Stack>
  );
}
