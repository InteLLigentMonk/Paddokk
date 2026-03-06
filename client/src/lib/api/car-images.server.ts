import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import {
  carImagesGetCarImages,
  carImagesUploadCarImage,
  carImagesDeleteCarImage,
  carImagesUpdateCarImage,
} from "@/generated/api/car-images/car-images";
import type { CarImagesResponse, CarImageDto } from "@/generated/api/schemas";

const carIdSchema = z.object({ carId: z.coerce.number() });

const deleteCarImageSchema = z.object({
  carId: z.coerce.number(),
  imageId: z.coerce.number(),
});

const updateCarImageSchema = z.object({
  carId: z.coerce.number(),
  imageId: z.coerce.number(),
  sortOrder: z.number().optional(),
  caption: z.string().nullable().optional(),
});

export const getCarImagesFn = createServerFn({ method: "GET" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    const result = await carImagesGetCarImages(carId);
    return result.data as CarImagesResponse;
  });

export const uploadCarImageFn = createServerFn({ method: "POST" })
  .inputValidator(
    z.object({
      carId: z.coerce.number(),
      file: z.instanceof(File),
    }),
  )
  .handler(async ({ data: { carId, file } }) => {
    const result = await carImagesUploadCarImage(carId, { File: file });
    return result.data as CarImageDto;
  });

export const deleteCarImageFn = createServerFn({ method: "POST" })
  .inputValidator(deleteCarImageSchema)
  .handler(async ({ data: { carId, imageId } }) => {
    await carImagesDeleteCarImage(carId, imageId);
  });

export const updateCarImageFn = createServerFn({ method: "POST" })
  .inputValidator(updateCarImageSchema)
  .handler(async ({ data: { carId, imageId, ...body } }) => {
    const result = await carImagesUpdateCarImage(carId, imageId, body);
    return result.data as CarImageDto;
  });
