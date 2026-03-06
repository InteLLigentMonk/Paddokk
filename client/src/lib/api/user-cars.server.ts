import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import {
  userCarsGetUserCars,
  userCarsGetUserCar,
  userCarsCreateUserCar,
  userCarsUpdateUserCar,
  userCarsDeleteUserCar,
} from "@/generated/api/user-cars/user-cars";
import type { UserCarsResponse, UserCarDto, CreateUserCarCommand } from "@/generated/api/schemas";

const carIdSchema = z.object({ carId: z.coerce.number() });

const createUserCarSchema = z.object({
  carMakeId: z.coerce.number(),
  carModelId: z.coerce.number(),
  carGenerationId: z.coerce.number().nullable(),
  year: z.coerce.number(),
  nickname: z.string().nullable(),
  color: z.string().nullable(),
  description: z.string().nullable(),
  isPrimary: z.boolean().optional(),
});

const updateUserCarSchema = z.object({
  carId: z.coerce.number(),
  nickname: z.string().nullable(),
  color: z.string().nullable(),
  description: z.string().nullable(),
  isPrimary: z.boolean().nullable(),
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
  .handler(async ({ data: { carId, ...body } }) => {
    const result = await userCarsUpdateUserCar(carId, { carId, ...body });
    return result.data as UserCarDto;
  });

export const deleteUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(carIdSchema)
  .handler(async ({ data: { carId } }) => {
    await userCarsDeleteUserCar(carId);
  });
