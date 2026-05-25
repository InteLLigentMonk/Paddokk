import { createServerFn } from "@tanstack/react-start";
import {
  UsersGetCarJourneysParams,
  UsersGetCarJourneysQueryParams,
  UsersGetJourneyBySlugParams,
  UsersGetUserByUsernameParams,
  UsersGetUserCarBySlugParams,
  UsersGetUserCarsByUsernameParams,
  UsersGetUserJourneysByUsernameParams,
} from "@/generated/api-zod/users/users.zod";
import {
  usersGetCarJourneys,
  usersGetCurrentUser,
  usersGetJourneyBySlug,
  usersGetUserByUsername,
  usersGetUserCarBySlug,
  usersGetUserCarsByUsername,
  usersGetUserJourneysByUsername,
} from "@/generated/api/users/users";

const getCarJourneysSchema = UsersGetCarJourneysParams.extend(
  UsersGetCarJourneysQueryParams.shape,
);

export const getCurrentUserFn = createServerFn({ method: "GET" }).handler(
  async () => await usersGetCurrentUser(),
);

export const getUserByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetUserByUsernameParams)
  .handler(
    async ({ data: { username } }) => await usersGetUserByUsername(username),
  );

export const getUserCarsByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(UsersGetUserCarsByUsernameParams)
  .handler(
    async ({ data: { username } }) =>
      await usersGetUserCarsByUsername(username),
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
