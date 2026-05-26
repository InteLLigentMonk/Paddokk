import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  changeUsernameFn,
  deleteCurrentUserFn,
  getCarJourneysFn,
  getCurrentUserFn,
  getUserByUsernameFn,
  getUserCarBySlugFn,
  getUserCarsByUsernameFn,
  getUserJourneyBySlugFn,
  getUserJourneysByUsernameFn,
  updateCurrentUserFn,
} from "./users";
import { getCarImagesFn } from "./car-images";

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

export const carJourneysQueryOptions = (
  username: string,
  carSlug: string,
  page = 1,
  pageSize = 5,
) =>
  queryOptions({
    queryKey: ["car-journeys", username, carSlug, page, pageSize],
    queryFn: () =>
      getCarJourneysFn({ data: { username, carSlug, page, pageSize } }),
  });

export function useUpdateCurrentUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateCurrentUserFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["current-user"] });
      queryClient.invalidateQueries({ queryKey: ["user-by-username"] });
    },
  });
}

export function useChangeUsername() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: changeUsernameFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["current-user"] });
      queryClient.invalidateQueries({ queryKey: ["user-by-username"] });
    },
  });
}

export function useDeleteCurrentUser() {
  return useMutation({
    mutationFn: deleteCurrentUserFn,
  });
}
