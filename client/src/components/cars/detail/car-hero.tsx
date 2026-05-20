import { Avatar, Badge, Box, Group, Text } from "@mantine/core";
import { Link } from "@tanstack/react-router";
import { MapPin, Zap } from "lucide-react";
import type { UserCarDto, CarImageDto } from "@/generated/api/schemas";
import { DRIVE_LABELS } from "./car-drive-select";
import { colorHexFromLabel } from "./car-color-swatch-input";
import { eraFromYear } from "./era";

interface CarHeroProps {
  car: UserCarDto;
  primaryImage: CarImageDto | undefined;
}

export function CarHero({ car, primaryImage }: CarHeroProps) {
  const imageUrl = primaryImage?.imageUrl ?? car.primaryImageUrl;
  const era = eraFromYear(car.year);
  const drive = car.drive != null ? DRIVE_LABELS[car.drive as number] : null;
  const colorHex = colorHexFromLabel(car.color);

  const displayName =
    car.nickname ||
    (car.isCustomBuild
      ? (car.customBuildName ?? "Custom Build")
      : [car.carMakeName, car.carModelName].filter(Boolean).join(" "));

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
        top={0}
        left={0}
        w={8}
        h="100%"
        style={{
          backgroundImage:
            "repeating-linear-gradient(45deg, var(--mantine-color-myColor-6) 0 6px, transparent 6px 12px)",
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
            <Avatar src={car.ownerAvatarUrl} size={24} radius="xl" />
            <Text fz={12} fw={600} c="white">
              {car.ownerUsername}
            </Text>
          </Group>
        </Link>
      </Box>

      {/* Bottom title block */}
      <Box pos="absolute" bottom={0} left={0} right={0} px={{ base: 20, md: 36 }} pb={28} style={{ zIndex: 3 }}>
        {/* Metadata row */}
        <Group gap={8} mb={8} wrap="wrap">
          {car.year && (
            <Text
              ff="monospace"
              fz={12}
              fw={700}
              c="rgba(255,255,255,0.7)"
              tt="uppercase"
              lts="0.1em"
            >
              {String(car.year)}
            </Text>
          )}
          {car.region && (
            <Badge
              variant="outline"
              color="gray.3"
              size="sm"
              leftSection={<MapPin size={10} />}
              style={{ borderColor: "rgba(255,255,255,0.4)", color: "rgba(255,255,255,0.8)" }}
            >
              {car.region}
            </Badge>
          )}
          {drive && (
            <Badge
              variant="outline"
              color="gray.3"
              size="sm"
              leftSection={<Zap size={10} />}
              style={{ borderColor: "rgba(255,255,255,0.4)", color: "rgba(255,255,255,0.8)" }}
            >
              {drive}
              {car.engine ? ` · ${car.engine}` : ""}
            </Badge>
          )}
          {!drive && car.engine && (
            <Badge
              variant="outline"
              color="gray.3"
              size="sm"
              leftSection={<Zap size={10} />}
              style={{ borderColor: "rgba(255,255,255,0.4)", color: "rgba(255,255,255,0.8)" }}
            >
              {car.engine}
            </Badge>
          )}
        </Group>

        {/* Main title */}
        <Text
          component="h1"
          m={0}
          c="white"
          lh={1.05}
          style={{
            fontSize: "clamp(40px, 6vw, 76px)",
            fontFamily: "Yapari",
            fontWeight: 400,
          }}
        >
          {displayName}
        </Text>

        {/* Sub-line: nickname + color swatch + era */}
        <Group gap={10} mt={8} align="center" wrap="wrap">
          {car.nickname && (car.isCustomBuild ? car.customBuildName : null) && (
            <Text fz={14} c="rgba(255,255,255,0.65)">
              {car.isCustomBuild ? car.customBuildName : null}
            </Text>
          )}
          {colorHex && (
            <Group gap={5} align="center">
              <Box
                w={12}
                h={12}
                style={{
                  borderRadius: "50%",
                  background: colorHex,
                  border: "1.5px solid rgba(255,255,255,0.4)",
                  flexShrink: 0,
                }}
              />
              <Text fz={12} c="rgba(255,255,255,0.65)">
                {car.color}
              </Text>
            </Group>
          )}
          {!colorHex && car.color && (
            <Text fz={12} c="rgba(255,255,255,0.65)">
              {car.color}
            </Text>
          )}
          {era && (
            <Text
              ff="monospace"
              fz={10}
              fw={700}
              tt="uppercase"
              lts="0.12em"
              c="rgba(255,255,255,0.5)"
            >
              {era}
            </Text>
          )}
        </Group>

        {/* Hero stats */}
        <Group gap={24} mt={14}>
          <Box>
            <Text ff="monospace" fz={20} fw={700} c="white" lh={1}>
              {Number(car.journeyCount)}
            </Text>
            <Text ff="monospace" fz={10} tt="uppercase" lts="0.1em" c="rgba(255,255,255,0.5)">
              Journeys
            </Text>
          </Box>
          <Box>
            <Text ff="monospace" fz={20} fw={700} c="white" lh={1}>
              {Number(car.likeCount)}
            </Text>
            <Text ff="monospace" fz={10} tt="uppercase" lts="0.1em" c="rgba(255,255,255,0.5)">
              Likes
            </Text>
          </Box>
        </Group>
      </Box>
    </Box>
  );
}
