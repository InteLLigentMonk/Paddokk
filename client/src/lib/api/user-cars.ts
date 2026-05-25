import { createServerFn } from "@tanstack/react-start";
import {
  UserCarsCreateUserCarBody,
  UserCarsDeleteUserCarParams,
  UserCarsGetUserCarParams,
  UserCarsLikeUserCarParams,
  UserCarsSubscribeToUserCarParams,
  UserCarsUnlikeUserCarParams,
  UserCarsUnsubscribeFromUserCarParams,
  UserCarsUpdateUserCarBody,
  UserCarsUpdateUserCarParams,
} from "@/generated/api-zod/user-cars/user-cars.zod";
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

// UpdateUserCarCommand is a full-replace shape on the wire, but the UI does
// per-section partial edits. Body fields are optional; carId from Params is
// re-applied on top so it stays required.
const updateUserCarSchema = UserCarsUpdateUserCarBody.partial().extend(
  UserCarsUpdateUserCarParams.shape,
);

export const getUserCarsFn = createServerFn({ method: "GET" }).handler(
  async () => await userCarsGetUserCars(),
);

export const getUserCarFn = createServerFn({ method: "GET" })
  .inputValidator(UserCarsGetUserCarParams)
  .handler(async ({ data: { carId } }) => await userCarsGetUserCar(carId));

export const createUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsCreateUserCarBody)
  .handler(async ({ data }) => await userCarsCreateUserCar(data));

export const updateUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(updateUserCarSchema)
  .handler(
    async ({ data }) =>
      await userCarsUpdateUserCar(data.carId, {
        carId: data.carId,
        customBuildName: data.customBuildName ?? null,
        nickname: data.nickname ?? null,
        color: data.color ?? null,
        region: data.region ?? null,
        drive: data.drive ?? null,
        engine: data.engine ?? null,
        odometerKm: data.odometerKm ?? null,
        ownerNote: data.ownerNote ?? null,
        specsByCategory: data.specsByCategory ?? null,
        isPrimary: data.isPrimary ?? null,
      }),
  );

export const deleteUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsDeleteUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsDeleteUserCar(carId);
  });

export const likeUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsLikeUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsLikeUserCar(carId);
  });

export const unlikeUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsUnlikeUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsUnlikeUserCar(carId);
  });

export const subscribeToUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsSubscribeToUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsSubscribeToUserCar(carId);
  });

export const unsubscribeFromUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsUnsubscribeFromUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsUnsubscribeFromUserCar(carId);
  });
