import { Avatar, Badge, Box, Group, Text, Title } from "@mantine/core";
import { Link } from "@tanstack/react-router";
import { DotIcon, MapPin, Zap } from "lucide-react";
import { DRIVE_LABELS } from "./car-drive-select";
import { colorLabelFromHex } from "./car-color-swatch-input";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";
import type { CarImageDto, UserCarDto } from "@/generated/api/schemas";

interface CarHeroProps {
  car: UserCarDto;
  primaryImage: CarImageDto | undefined;
}

export function CarHero({ car, primaryImage }: CarHeroProps) {
  const rawImageUrl = primaryImage?.imageUrl ?? car.primaryImageUrl;
  const imageUrl = optimizeImageUrl(rawImageUrl, 1600);
  const drive = car.drive != null ? DRIVE_LABELS[car.drive] : null;
  const colorHex = car.color ?? null;
  const colorLabel = colorLabelFromHex(car.color);

  const mainTitle = car.isCustomBuild
    ? (car.customBuildName ?? "Custom Build")
    : [car.carMakeName, car.carModelName, car.carGenerationName]
        .filter(Boolean)
        .join(" ");

  return (
    <Box
      pos="relative"
      style={{
        minHeight: 380,
        height: "min(54vh, 520px)",
        overflow: "hidden",
        background: imageUrl
          ? `url(${imageUrl}) center/cover no-repeat`
          : "light-dark(var(--mantine-color-dark-7), var(--mantine-color-dark-8))",
      }}
    >
      {/* Vignette gradient */}
      <Box
        pos="absolute"
        style={{
          inset: 0,
          background:
            "linear-gradient(to top, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.35) 50%, rgba(0,0,0,0.15) 100%)",
          zIndex: 1,
        }}
      />

      {/* Yellow hash-stripe accent top-left */}
      <Box
        pos="absolute"
        top={30}
        left={0}
        w={150}
        h={12}
        style={{
          backgroundImage:
            "repeating-linear-gradient(45deg, var(--mantine-primary-color-6) 0 6px, black 6px 12px)",
          zIndex: 2,
        }}
      />

      {/* Owner pill top-right */}
      <Box pos="absolute" top={16} right={20} style={{ zIndex: 3 }}>
        <Link
          to="/users/$username"
          params={{ username: car.ownerUsername }}
          style={{ textDecoration: "none" }}
        >
          <Group
            gap={8}
            px={12}
            py={6}
            style={{
              background: "rgba(0,0,0,0.55)",
              backdropFilter: "blur(8px)",
              borderRadius: "var(--mantine-radius-xl)",
            }}
          >
            <Avatar src={optimizeImageUrl(car.ownerAvatarUrl, 80)} size={24} radius="xl" />
            <Text fz={12} fw={600} c="white">
              {car.ownerUsername}
            </Text>
          </Group>
        </Link>
      </Box>

      {/* Bottom title block */}
      <Box
        pos="absolute"
        bottom={0}
        left={0}
        right={0}
        px={{ base: 20, md: 36 }}
        pb={28}
        style={{ zIndex: 3 }}
      >
        <Group align="flex-end" justify="space-between" wrap="nowrap">
          {/* Left: metadata + title + sub-line */}
          <Box style={{ flex: 1, minWidth: 0 }}>
            {/* Metadata row */}
            <Group gap={8} mb={8} wrap="wrap">
              {car.year && (
                <Text
                  ff="monospace"
                  fz={12}
                  fw={700}
                  c="var(--mantine-primary-color-5)"
                  tt="uppercase"
                  lts="0.1em"
                >
                  {String(car.year)}
                </Text>
              )}
              {car.year && (car.region || car.engine || drive) && (
                <DotIcon color="var(--mantine-color-gray-3" />
              )}
              {car.region && (
                <Badge
                  variant="default"
                  c="var(--mantine-color-gray-2)"
                  bg="rgba(from var(--mantine-color-white) r g b / 0.2)"
                  bd={0}
                  size="md"
                  leftSection={<MapPin size={12} />}
                >
                  {car.region}
                </Badge>
              )}
              {drive && (
                <Badge
                  variant="default"
                  c="var(--mantine-color-gray-2)"
                  bg="rgba(from var(--mantine-color-white) r g b / 0.2)"
                  bd={0}
                  size="md"
                  leftSection={<Zap size={10} />}
                >
                  {drive}
                  {car.engine ? ` · ${car.engine}` : ""}
                </Badge>
              )}
              {!drive && car.engine && (
                <Badge
                  variant="default"
                  c="var(--mantine-color-gray-2)"
                  bg="rgba(from var(--mantine-color-white) r g b / 0.2)"
                  bd={0}
                  size="md"
                  leftSection={<Zap size={10} />}
                >
                  {car.engine}
                </Badge>
              )}
            </Group>

            {/* Main title */}
            <Title
              order={1}
              fz={{ base: "d3", md: "d2", lg: "d1" }}
              c="white"
              lh={1.05}
            >
              {mainTitle}
            </Title>

            {/* Sub-line: nickname (replaces era) + color swatch */}
            <Group gap={10} mt={8} align="center" wrap="wrap">
              {car.nickname && (
                <Text fz="lg" c="var(--mantine-color-gray-4)" ta="center">
                  "{car.nickname}"
                </Text>
              )}
              {colorHex && (
                <Group gap={5} align="center">
                  <Box
                    w={18}
                    h={18}
                    style={{
                      borderRadius: "50%",
                      background: colorHex,
                      border: "2px solid var(--mantine-color-white)",
                      flexShrink: 0,
                    }}
                  />
                  <Text fz={12} c="var(--mantine-color-gray-4)">
                    {colorLabel ?? car.color}
                  </Text>
                </Group>
              )}
              {!colorHex && car.color && (
                <Text fz={12} c="var(--mantine-color-gray-4)">
                  {car.color}
                </Text>
              )}
            </Group>

            {/* Stats on mobile */}
            <Group gap={24} mt={14} hiddenFrom="md">
              <Box>
                <Text ff="monospace" fz={20} fw={700} c="white" lh={1}>
                  {Number(car.journeyCount)}
                </Text>
                <Text
                  ff="monospace"
                  fz={10}
                  tt="uppercase"
                  lts="0.1em"
                  c="rgba(255,255,255,0.5)"
                >
                  Journeys
                </Text>
              </Box>
              <Box>
                <Text ff="monospace" fz={20} fw={700} c="white" lh={1}>
                  {Number(car.likeCount)}
                </Text>
                <Text
                  ff="monospace"
                  fz={10}
                  tt="uppercase"
                  lts="0.1em"
                  c="rgba(255,255,255,0.5)"
                >
                  Likes
                </Text>
              </Box>
            </Group>
          </Box>

          {/* Stats on md+ — right side */}
          <Group
            gap={32}
            visibleFrom="md"
            align="flex-end"
            style={{ flexShrink: 0 }}
          >
            <Box ta="right">
              <Text ff="Yapari" fz="d3" c="white" lh={1}>
                {Number(car.journeyCount)}
              </Text>
              <Text
                ff="monospace"
                fz={10}
                tt="uppercase"
                lts="0.1em"
                c="var(--mantine-color-gray-5)"
              >
                Journeys
              </Text>
            </Box>
            <Box ta="right">
              <Text ff="Yapari" fz="d3" c="white" lh={1}>
                {Number(car.likeCount)}
              </Text>
              <Text
                ff="monospace"
                fz={10}
                tt="uppercase"
                lts="0.1em"
                c="var(--mantine-color-gray-5)"
              >
                Likes
              </Text>
            </Box>
          </Group>
        </Group>
      </Box>
    </Box>
  );
}
