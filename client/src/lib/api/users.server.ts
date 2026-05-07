import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import {
  usersGetCurrentUser,
  usersGetUserByUsername,
  usersGetUserCarsByUsername,
  usersGetUserCarBySlug,
  usersGetUserJourneysByUsername,
  usersGetJourneyBySlug,
} from "@/generated/api/users/users";
import type {
  UserDto,
  UserCarDto,
  JourneyDto,
} from "@/generated/api/schemas";

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
    return result.data as UserCarDto[];
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
    return result.data as JourneyDto[];
  });

export const getUserJourneyBySlugFn = createServerFn({ method: "GET" })
  .inputValidator(usernameSlugSchema)
  .handler(async ({ data: { username, slug } }) => {
    const result = await usersGetJourneyBySlug(username, slug);
    return result.data as JourneyDto;
  });
