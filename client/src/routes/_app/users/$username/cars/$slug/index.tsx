import { createFileRoute, notFound } from "@tanstack/react-router";
import { z } from "zod";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import { getUserCarBySlugFn } from "@/lib/api/users.server";
import { getCarImagesFn } from "@/lib/api/car-images.server";

const searchSchema = z.object({ edit: z.boolean().optional() });

export const Route = createFileRoute("/_app/users/$username/cars/$slug/")({
  validateSearch: searchSchema,
  loader: async ({ params }) => {
    try {
      const car = await getUserCarBySlugFn({
        data: { username: params.username, slug: params.slug },
      });
      const imagesResponse = await getCarImagesFn({ data: { carId: Number(car.id) } });
      return { car, images: imagesResponse?.images ?? [] };
    } catch {
      throw notFound();
    }
  },
  component: UserCarDetailRoute,
});

function UserCarDetailRoute() {
  const { car, images } = Route.useLoaderData();
  const { edit } = Route.useSearch();
  return <CarDetailPage car={car} images={images} startInEditMode={!!edit} />;
}
