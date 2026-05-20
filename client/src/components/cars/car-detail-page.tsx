import { Box, Container, Grid, Stack } from "@mantine/core";
import type { UserCarDto, CarImageDto } from "@/generated/api/schemas";
import { CarHero } from "./detail/car-hero";
import { CarSpecStrip } from "./detail/car-spec-strip";
import { CarActionBar } from "./detail/car-action-bar";
import { CarOwnerNote } from "./detail/car-owner-note";
import { CarSpecSheet } from "./detail/car-spec-sheet";
import { CarJourneyList } from "./detail/car-journey-list";
import { CarPhotosSection } from "./detail/car-photos-section";
import { CarVitalsCard } from "./detail/car-vitals-card";
import { CarOwnerGarage } from "./detail/car-owner-garage";

interface CarDetailPageProps {
  car: UserCarDto;
  images: CarImageDto[];
}

export function CarDetailPage({ car, images }: CarDetailPageProps) {
  const primaryImage = images.find((img) => img.isPrimary) ?? images[0];

  return (
    <>
      <CarHero car={car} primaryImage={primaryImage} />
      <CarSpecStrip car={car} />

      <Container size="xl" py="xl">
        <Grid gap="xl" align="flex-start">
          <Grid.Col span={{ base: 12, md: 8 }}>
            <Stack gap={36}>
              <CarActionBar car={car} />
              <CarOwnerNote car={car} />
              <CarSpecSheet car={car} />
              <CarJourneyList
                username={car.ownerUsername}
                slug={car.slug}
                isOwner={car.isOwner}
                totalCount={car.journeyCount}
              />
              <CarPhotosSection car={car} images={images} />
            </Stack>
          </Grid.Col>

          <Grid.Col span={{ base: 12, md: 4 }}>
            <Box
              style={{
                position: "sticky",
                top: 80,
              }}
            >
              <Stack gap="md">
                <CarVitalsCard car={car} />
                <CarOwnerGarage car={car} />
              </Stack>
            </Box>
          </Grid.Col>
        </Grid>
      </Container>
    </>
  );
}
