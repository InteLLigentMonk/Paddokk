import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import {
  userCarsGetUserCars,
  userCarsGetUserCar,
  userCarsCreateUserCar,
  userCarsUpdateUserCar,
  userCarsDeleteUserCar,
  userCarsLikeUserCar,
  userCarsUnlikeUserCar,
  userCarsSubscribeToUserCar,
  userCarsUnsubscribeFromUserCar,
} from "@/generated/api/user-cars/user-cars";
import type { UserCarsResponse, UserCarDto, CreateUserCarCommand } from "@/generated/api/schemas";

const carIdSchema = z.object({ carId: z.coerce.number() });

const createUserCarSchema = z.object({
  isCustomBuild: z.boolean().default(false),
  customBuildName: z.string().nullable().optional(),
  carMakeId: z.coerce.number().nullable().optional(),
  carModelId: z.coerce.number().nullable().optional(),
  carGenerationId: z.coerce.number().nullable().optional(),
  year: z.coerce.number().nullable().optional(),
  nickname: z.string().nullable().optional(),
  color: z.string().nullable().optional(),
  description: z.string().nullable().optional(),
  isPrimary: z.boolean().optional(),
});

const updateUserCarSchema = z.object({
  carId: z.coerce.number(),
  customBuildName: z.string().nullable().optional(),
  nickname: z.string().nullable().optional(),
  color: z.string().nullable().optional(),
  description: z.string().nullable().optional(),
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
  .handler(async ({ data: { carId, customBuildName, nickname, color, description, isPrimary } }) => {
    const result = await userCarsUpdateUserCar(carId, {
      carId,
      customBuildName: customBuildName ?? null,
      nickname: nickname ?? null,
      color: color ?? null,
      description: description ?? null,
      isPrimary: isPrimary ?? null,
    } as import("@/generated/api/schemas").UpdateUserCarCommand);
    return result.data as UserCarDto;
  });

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
