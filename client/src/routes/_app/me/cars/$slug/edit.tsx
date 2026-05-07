import { createFileRoute, notFound, redirect } from "@tanstack/react-router";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import {
  getCurrentUserFn,
  getUserCarBySlugFn,
} from "@/lib/api/users.server";
import { getCarImagesFn } from "@/lib/api/car-images.server";

export const Route = createFileRoute("/_app/me/cars/$slug/edit")({
  loader: async ({ params }) => {
    const me = await getCurrentUserFn();
    if (!me?.username) throw redirect({ to: "/login" });

    try {
      const car = await getUserCarBySlugFn({
        data: { username: me.username, slug: params.slug },
      });
      const imagesResponse = await getCarImagesFn({ data: { carId: Number(car.id) } });
      return { car, images: imagesResponse?.images ?? [] };
    } catch {
      throw notFound();
    }
  },
  component: EditCarRoute,
});

function EditCarRoute() {
  const { car, images } = Route.useLoaderData();
  return <CarDetailPage car={car} images={images} startInEditMode={true} />;
}
