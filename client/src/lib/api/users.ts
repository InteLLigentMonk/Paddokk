import { createServerFn } from "@tanstack/react-start";
import {
  UsersChangeCurrentUsernameBody,
  UsersGetCarJourneysParams,
  UsersGetCarJourneysQueryParams,
  UsersGetJourneyBySlugParams,
  UsersGetUserByUsernameParams,
  UsersGetUserCarBySlugParams,
  UsersGetUserCarsByUsernameParams,
  UsersGetUserCarsByUsernameQueryParams,
  UsersGetUserJourneysByUsernameParams,
  UsersUpdateCurrentUserBody,
} from "@/generated/api-zod/users/users.zod";
import {
  usersChangeCurrentUsername,
  usersDeleteCurrentUser,
  usersGetCarJourneys,
  usersGetCurrentUser,
  usersGetJourneyBySlug,
  usersGetUserByUsername,
  usersGetUserCarBySlug,
  usersGetUserCarsByUsername,
  usersGetUserJourneysByUsername,
  usersUpdateCurrentUser,
} from "@/generated/api/users/users";

const getCarJourneysSchema = UsersGetCarJourneysParams.extend(
  UsersGetCarJourneysQueryParams.shape,
);

const getUserCarsByUsernameSchema = UsersGetUserCarsByUsernameParams.extend(
  UsersGetUserCarsByUsernameQueryParams.shape,
);

const updateCurrentUserSchema = UsersUpdateCurrentUserBody.partial();

export const getCurrentUserFn = createServerFn({ method: "GET" }).handler(
  async () => await usersGetCurrentUser(),
);

export const getUserByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetUserByUsernameParams)
  .handler(
    async ({ data: { username } }) => await usersGetUserByUsername(username),
  );

export const getUserCarsByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(getUserCarsByUsernameSchema)
  .handler(
    async ({ data: { username, limit } }) =>
      await usersGetUserCarsByUsername(username, { limit }),
  );

export const getUserCarBySlugFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetUserCarBySlugParams)
  .handler(
    async ({ data: { username, slug } }) =>
      await usersGetUserCarBySlug(username, slug),
  );

export const getUserJourneysByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetUserJourneysByUsernameParams)
  .handler(
    async ({ data: { username } }) =>
      await usersGetUserJourneysByUsername(username),
  );

export const getUserJourneyBySlugFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetJourneyBySlugParams)
  .handler(
    async ({ data: { username, slug } }) =>
      await usersGetJourneyBySlug(username, slug),
  );

export const getCarJourneysFn = createServerFn({ method: "GET" })
  .inputValidator(getCarJourneysSchema)
  .handler(
    async ({ data: { username, carSlug, page, pageSize } }) =>
      await usersGetCarJourneys(username, carSlug, { page, pageSize }),
  );

export const updateCurrentUserFn = createServerFn({ method: "POST" })
  .inputValidator(updateCurrentUserSchema)
  .handler(
    async ({ data }) =>
      await usersUpdateCurrentUser({
        firstName: data.firstName ?? null,
        lastName: data.lastName ?? null,
        displayName: data.displayName ?? null,
        bio: data.bio ?? null,
        avatarUrl: data.avatarUrl ?? null,
      }),
  );

export const changeUsernameFn = createServerFn({ method: "POST" })
  .inputValidator(UsersChangeCurrentUsernameBody)
  .handler(async ({ data }) => await usersChangeCurrentUsername(data));

export const deleteCurrentUserFn = createServerFn({ method: "POST" }).handler(
  async () => {
    await usersDeleteCurrentUser();
  },
);
