import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import type {
  CreateUserCarCommand,
  UpdateUserCarCommand,
  UserCarDto,
  UserCarsResponse,
} from "@/generated/api/schemas";
import {
  userCarsCreateUserCar,
  userCarsDeleteUserCar,
  userCarsGetUserCar,
  userCarsGetUserCars,
  userCarsLikeUserCar,
  userCarsSubscribeToUserCar,
  userCarsUnlikeUserCar,
  userCarsUnsubscribeFromUserCar,
  userCarsUpdateUserCar,
} from "@/generated/api/user-cars/user-cars";

const carIdSchema = z.object({ carId: z.coerce.number() });

const carSpecCategorySchema = z.object({
  category: z.string(),
  items: z.array(z.string()),
});

const createUserCarSchema = z.object({
  isCustomBuild: z.boolean().default(false),
  customBuildName: z.string().nullable().optional(),
  carMakeId: z.coerce.number().nullable().optional(),
  carModelId: z.coerce.number().nullable().optional(),
  carGenerationId: z.coerce.number().nullable().optional(),
  year: z.coerce.number().nullable().optional(),
  nickname: z.string().nullable().optional(),
  color: z.string().nullable().optional(),
  isPrimary: z.boolean().optional(),
});

const updateUserCarSchema = z.object({
  carId: z.coerce.number(),
  customBuildName: z.string().nullable().optional(),
  nickname: z.string().nullable().optional(),
  color: z.string().nullable().optional(),
  region: z.string().nullable().optional(),
  drive: z.number().nullable().optional(),
  engine: z.string().nullable().optional(),
  odometerKm: z.coerce.number().nullable().optional(),
  ownerNote: z.string().nullable().optional(),
  specsByCategory: z.array(carSpecCategorySchema).nullable().optional(),
  isPrimary: z.boolean().nullable().optional(),
});

export const getUserCarsFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await userCarsGetUserCars();
    return result.data as UserCarsResponse;
  },
);

export const getUserCarFn = createServerFn({ method: "GET" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    const result = await userCarsGetUserCar(carId);
    return result.data as UserCarDto;
  });

export const createUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(createUserCarSchema)
  .handler(async ({ data }) => {
    const result = await userCarsCreateUserCar(data as CreateUserCarCommand);
    return result.data as UserCarDto;
  });

export const updateUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(updateUserCarSchema)
  .handler(
    async ({
      data: {
        carId,
        customBuildName,
        nickname,
        color,
        region,
        drive,
        engine,
        odometerKm,
        ownerNote,
        specsByCategory,
        isPrimary,
      },
    }) => {
      const result = await userCarsUpdateUserCar(carId, {
        carId,
        customBuildName: customBuildName ?? null,
        nickname: nickname ?? null,
        color: color ?? null,
        region: region ?? null,
        drive: drive ?? null,
        engine: engine ?? null,
        odometerKm: odometerKm ?? null,
        ownerNote: ownerNote ?? null,
        specsByCategory: specsByCategory ?? null,
        isPrimary: isPrimary ?? null,
      } as UpdateUserCarCommand);
      return result.data as UserCarDto;
    },
  );

export const deleteUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsDeleteUserCar(carId);
  });

export const likeUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsLikeUserCar(carId);
  });

export const unlikeUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsUnlikeUserCar(carId);
  });

export const subscribeToUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsSubscribeToUserCar(carId);
  });

export const unsubscribeFromUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsUnsubscribeFromUserCar(carId);
  });
