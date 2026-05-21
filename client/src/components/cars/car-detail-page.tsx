import { Container, Grid, Stack } from "@mantine/core";
import { CarHero } from "./detail/car-hero";
import { CarSpecStrip } from "./detail/car-spec-strip";
import { CarActionBar } from "./detail/car-action-bar";
import { CarOwnerNote } from "./detail/car-owner-note";
import { CarSpecSheet } from "./detail/car-spec-sheet";
import { CarJourneyList } from "./detail/car-journey-list";
import { CarPhotosSection } from "./detail/car-photos-section";
import { CarVitalsCard } from "./detail/car-vitals-card";
import { CarOwnerGarage } from "./detail/car-owner-garage";
import type { CarImageDto, UserCarDto } from "@/generated/api/schemas";

interface CarDetailPageProps {
  car: UserCarDto;
  images: Array<CarImageDto>;
}

export function CarDetailPage({ car, images }: CarDetailPageProps) {
  const primaryImage = images.find((img) => img.isPrimary) ?? images[0];

  return (
    <>
      <CarHero car={car} primaryImage={primaryImage} />
      <CarSpecStrip car={car} />

      <Container size="xl" py="xl">
        <Grid gap="xl">
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
            <Stack
              gap="md"
              style={{
                position: "sticky",
                top: 32,
              }}
            >
              <CarVitalsCard car={car} />
              <CarOwnerGarage car={car} />
            </Stack>
          </Grid.Col>
        </Grid>
      </Container>
    </>
  );
}
