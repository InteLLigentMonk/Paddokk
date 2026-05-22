import { Avatar, Group, Stack, Text } from "@mantine/core";
import { Link } from "@tanstack/react-router";
import classes from "./owner-link.module.css";
import type { MantineRadius, MantineSize } from "@mantine/core";
import type { ReactNode } from "react";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";

export type OwnerLinkTarget =
  | { kind: "user"; username: string }
  | { kind: "car"; username: string; slug: string };

interface OwnerLinkProps {
  target: OwnerLinkTarget;
  avatarUrl?: string | null;
  primaryText: string;
  secondaryText?: ReactNode;
  avatarSize?: MantineSize;
  avatarRadius?: MantineRadius;
  primaryTextSize?: MantineSize;
  primaryTextWeight?: number;
  reverse?: boolean;
}

function OwnerContent({
  avatarUrl,
  primaryText,
  secondaryText,
  avatarSize,
  avatarRadius,
  primaryTextSize,
  primaryTextWeight,
  reverse,
}: Omit<OwnerLinkProps, "target">) {
  const avatar = (
    <Avatar
      src={optimizeImageUrl(avatarUrl, 80) ?? null}
      radius={avatarRadius ?? "xl"}
      size={avatarSize ?? "md"}
      alt={primaryText}
    />
  );
  const text = (
    <Stack
      gap={2}
      miw={0}
      style={reverse ? { alignItems: "flex-end" } : undefined}
    >
      <Text
        fw={primaryTextWeight ?? 600}
        size={primaryTextSize ?? "sm"}
        className={classes.primary}
        style={{ whiteSpace: "nowrap" }}
      >
        {primaryText}
      </Text>
      {secondaryText !== undefined &&
        secondaryText !== null &&
        secondaryText !== false && (
          <Text size="xs" c="dimmed">
            {secondaryText}
          </Text>
        )}
    </Stack>
  );

  return (
    <Group
      gap={reverse ? "xs" : "sm"}
      wrap="nowrap"
      align={reverse ? "flex-start" : "center"}
    >
      {reverse ? (
        <>
          {text}
          {avatar}
        </>
      ) : (
        <>
          {avatar}
          {text}
        </>
      )}
    </Group>
  );
}

export function OwnerLink(props: OwnerLinkProps) {
  const { target, reverse = false, ...rest } = props;
  const content = <OwnerContent {...rest} reverse={reverse} />;

  if (target.kind === "user") {
    return (
      <Link
        to="/users/$username"
        params={{ username: target.username }}
        className={classes.root}
      >
        {content}
      </Link>
    );
  }

  return (
    <Link
      to="/users/$username/cars/$slug"
      params={{ username: target.username, slug: target.slug }}
      className={classes.root}
    >
      {content}
    </Link>
  );
}
