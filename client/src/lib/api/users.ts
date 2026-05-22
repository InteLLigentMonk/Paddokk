import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import type { JourneyDto, UserCarDto, UserDto } from "@/generated/api/schemas";
import {
  usersGetCarJourneys,
  usersGetCurrentUser,
  usersGetJourneyBySlug,
  usersGetUserByUsername,
  usersGetUserCarBySlug,
  usersGetUserCarsByUsername,
  usersGetUserJourneysByUsername,
} from "@/generated/api/users/users";

export const getCurrentUserFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await usersGetCurrentUser();
    return result.data as UserDto;
  },
);

const usernameSchema = z.object({ username: z.string() });
const usernameSlugSchema = z.object({
  username: z.string(),
  slug: z.string(),
});
const carJourneysSchema = z.object({
  username: z.string(),
  carSlug: z.string(),
  page: z.number().optional(),
  pageSize: z.number().optional(),
});

export const getUserByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSchema)
  .handler(async ({ data: { username } }) => {
    const result = await usersGetUserByUsername(username);
    return result.data as UserDto;
  });

export const getUserCarsByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSchema)
  .handler(async ({ data: { username } }) => {
    const result = await usersGetUserCarsByUsername(username);
    return result.data as Array<UserCarDto>;
  });

export const getUserCarBySlugFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSlugSchema)
  .handler(async ({ data: { username, slug } }) => {
    const result = await usersGetUserCarBySlug(username, slug);
    return result.data as UserCarDto;
  });

export const getUserJourneysByUsernameFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSchema)
  .handler(async ({ data: { username } }) => {
    const result = await usersGetUserJourneysByUsername(username);
    return result.data as Array<JourneyDto>;
  });

export const getUserJourneyBySlugFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSlugSchema)
  .handler(async ({ data: { username, slug } }) => {
    const result = await usersGetJourneyBySlug(username, slug);
    return result.data as JourneyDto;
  });

export const getCarJourneysFn = createServerFn({ method: "GET" })
  .inputValidator(carJourneysSchema)
  .handler(async ({ data: { username, carSlug, page, pageSize } }) => {
    const result = await usersGetCarJourneys(username, carSlug, {
      page,
      pageSize,
    });
    return result.data as Array<JourneyDto>;
  });
