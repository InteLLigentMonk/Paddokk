import { queryOptions } from "@tanstack/react-query";
import {
  getCurrentUserFn,
  getUserByUsernameFn,
  getUserCarsByUsernameFn,
  getUserCarBySlugFn,
  getUserJourneysByUsernameFn,
  getUserJourneyBySlugFn,
} from "./users.server";
import { getCarImagesFn } from "./car-images.server";

export const currentUserQueryOptions = () =>
  queryOptions({
    queryKey: ["current-user"],
    queryFn: () => getCurrentUserFn(),
  });

export const userByUsernameQueryOptions = (username: string) =>
  queryOptions({
    queryKey: ["user-by-username", username],
    queryFn: () => getUserByUsernameFn({ data: { username } }),
  });

export const userCarsByUsernameQueryOptions = (username: string) =>
  queryOptions({
    queryKey: ["user-cars-by-username", username],
    queryFn: () => getUserCarsByUsernameFn({ data: { username } }),
  });

export const userCarBySlugQueryOptions = (username: string, slug: string) =>
  queryOptions({
    queryKey: ["user-car-by-slug", username, slug],
    queryFn: () => getUserCarBySlugFn({ data: { username, slug } }),
  });

export const userJourneysByUsernameQueryOptions = (username: string) =>
  queryOptions({
    queryKey: ["user-journeys-by-username", username],
    queryFn: () => getUserJourneysByUsernameFn({ data: { username } }),
  });

export const userJourneyBySlugQueryOptions = (username: string, slug: string) =>
  queryOptions({
    queryKey: ["journey-by-slug", username, slug],
    queryFn: () => getUserJourneyBySlugFn({ data: { username, slug } }),
  });

export const carImagesQueryOptions = (carId: number) =>
  queryOptions({
    queryKey: ["car-images", carId],
    queryFn: () => getCarImagesFn({ data: { carId } }),
  });
