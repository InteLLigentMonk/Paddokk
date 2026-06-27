import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  changeUsernameFn,
  deleteCurrentUserFn,
  followUserFn,
  getCarJourneysFn,
  getCurrentUserFn,
  getFollowersFn,
  getFollowingFn,
  getUserByUsernameFn,
  getUserCarBySlugFn,
  getUserCarsByUsernameFn,
  getUserJourneyBySlugFn,
  getUserJourneysByUsernameFn,
  unfollowUserFn,
  updateCurrentUserFn,
} from "./users";
import { getCarImagesFn } from "./car-images";
import { requirePage } from "./infinite";
import { userKeys } from "./users.keys";
import { carKeys } from "./cars.keys";
import type { FollowListType } from "./users.keys";
import type { UserDto } from "@/generated/api/schemas";

export type { FollowListType };

export const currentUserQueryOptions = () =>
  queryOptions({
    queryKey: userKeys.currentUser,
    queryFn: () => getCurrentUserFn(),
  });

export const userByUsernameQueryOptions = (username: string) =>
  queryOptions({
    queryKey: userKeys.userDetail(username),
    queryFn: () => getUserByUsernameFn({ data: { username } }),
  });

export const userCarsByUsernameQueryOptions = (
  username: string,
  limit?: number,
) =>
  queryOptions({
    queryKey: userKeys.userCars(username, limit),
    queryFn: () => getUserCarsByUsernameFn({ data: { username, limit } }),
  });

export const userCarBySlugQueryOptions = (username: string, slug: string) =>
  queryOptions({
    queryKey: userKeys.userCarDetail(username, slug),
    queryFn: () => getUserCarBySlugFn({ data: { username, slug } }),
  });

export const userJourneysByUsernameQueryOptions = (username: string) =>
  queryOptions({
    queryKey: userKeys.userJourneys(username),
    queryFn: () => getUserJourneysByUsernameFn({ data: { username } }),
  });

export const userJourneyBySlugQueryOptions = (username: string, slug: string) =>
  queryOptions({
    queryKey: userKeys.journeyDetail(username, slug),
    queryFn: () => getUserJourneyBySlugFn({ data: { username, slug } }),
  });

const FOLLOW_LIST_PAGE_SIZE = 20;

export const followListInfiniteQueryOptions = (
  userId: string,
  type: FollowListType,
) =>
  infiniteQueryOptions({
    queryKey: userKeys.followList(type, userId),
    queryFn: ({ pageParam }) => {
      const data = {
        id: userId,
        page: pageParam,
        pageSize: FOLLOW_LIST_PAGE_SIZE,
      };
      return requirePage(
        type === "followers"
          ? getFollowersFn({ data })
          : getFollowingFn({ data }),
      );
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasNextPage ? allPages.length + 1 : undefined,
  });

export const carImagesQueryOptions = (carId: number) =>
  queryOptions({
    queryKey: carKeys.carImages(carId),
    queryFn: () => getCarImagesFn({ data: { carId } }),
  });

export const carJourneysQueryOptions = (
  username: string,
  carSlug: string,
  page = 1,
  pageSize = 5,
) =>
  queryOptions({
    queryKey: carKeys.carJourneys(username, carSlug, page, pageSize),
    queryFn: () =>
      getCarJourneysFn({ data: { username, carSlug, page, pageSize } }),
  });

export function useUpdateCurrentUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateCurrentUserFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.currentUser });
      queryClient.invalidateQueries({ queryKey: userKeys.userDetailRoot });
    },
  });
}

export function useChangeUsername() {
  const queryClient = useQueryClient();
  return useMutation({
    // The form renders the failure inline (field-level codes), so opt out of the toast.
    meta: { suppressGlobalError: true },
    mutationFn: changeUsernameFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.currentUser });
      queryClient.invalidateQueries({ queryKey: userKeys.userDetailRoot });
    },
  });
}

export function useDeleteCurrentUser() {
  return useMutation({
    mutationFn: deleteCurrentUserFn,
  });
}

/**
 * Optimistically toggles the follow relationship for a profile, patching the
 * cached UserDto (isFollowedByMe + followerCount) immediately and rolling back
 * to the prior snapshot if the server rejects the write.
 */
export function useToggleFollow(userId: string, username: string) {
  const queryClient = useQueryClient();
  const queryKey = userKeys.userDetail(username);

  return useMutation({
    mutationFn: (isCurrentlyFollowing: boolean) =>
      isCurrentlyFollowing
        ? unfollowUserFn({ data: { id: userId } })
        : followUserFn({ data: { id: userId } }),
    onMutate: async (isCurrentlyFollowing) => {
      await queryClient.cancelQueries({ queryKey });
      const previous = queryClient.getQueryData<UserDto>(queryKey);
      if (previous) {
        queryClient.setQueryData<UserDto>(queryKey, {
          ...previous,
          isFollowedByMe: !isCurrentlyFollowing,
          followerCount:
            previous.followerCount + (isCurrentlyFollowing ? -1 : 1),
        });
      }
      return { previous };
    },
    onError: (_err, _isCurrentlyFollowing, context) => {
      // Roll back the optimistic patch; the global mutation handler shows the toast.
      if (context?.previous) {
        queryClient.setQueryData(queryKey, context.previous);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey });
      // Refresh any open followers/following lists so a follow-back from a row
      // reflects the new relationship once the write settles.
      queryClient.invalidateQueries({ queryKey: userKeys.followListRoot });
    },
  });
}
