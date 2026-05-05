import { createFileRoute, redirect } from "@tanstack/react-router";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import { getUserCarFn } from "@/lib/api/user-cars.server";
import { getCarImagesFn } from "@/lib/api/car-images.server";
import z from "zod";

const searchSchema = z.object({ edit: z.boolean().optional() });

export const Route = createFileRoute("/_app/me/cars/$carId/")({
  validateSearch: searchSchema,
  loader: async ({ params }) => {
    const carId = Number(params.carId);
    const [car, imagesResponse] = await Promise.all([
      getUserCarFn({ data: { carId } }).catch(() => {
        throw redirect({ to: "/me/cars" });
      }),
      getCarImagesFn({ data: { carId } }),
    ]);
    return { car, images: imagesResponse?.images ?? [] };
  },
  component: CarDetailRoute,
});

function CarDetailRoute() {
  const { car, images } = Route.useLoaderData();
  const { edit } = Route.useSearch();
  return <CarDetailPage car={car} images={images} startInEditMode={!!edit} />;
}
