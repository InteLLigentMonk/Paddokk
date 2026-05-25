import { createServerFn } from "@tanstack/react-start";
import {
  CarImagesDeleteCarImageParams,
  CarImagesGetCarImagesParams,
  CarImagesUpdateCarImageBody,
  CarImagesUpdateCarImageParams,
} from "@/generated/api-zod/car-images/car-images.zod";
import {
  carImagesDeleteCarImage,
  carImagesGetCarImages,
  carImagesUpdateCarImage,
} from "@/generated/api/car-images/car-images";

const updateCarImageSchema = CarImagesUpdateCarImageParams.extend(
  CarImagesUpdateCarImageBody.shape,
);

export const getCarImagesFn = createServerFn({ method: "GET" })
  .inputValidator(CarImagesGetCarImagesParams)
  .handler(async ({ data: { carId } }) => await carImagesGetCarImages(carId));

export const deleteCarImageFn = createServerFn({ method: "POST" })
  .inputValidator(CarImagesDeleteCarImageParams)
  .handler(async ({ data: { carId, imageId } }) => {
    await carImagesDeleteCarImage(carId, imageId);
  });

export const updateCarImageFn = createServerFn({ method: "POST" })
  .inputValidator(updateCarImageSchema)
  .handler(
    async ({ data: { carId, imageId, ...body } }) =>
      await carImagesUpdateCarImage(carId, imageId, body),
  );
