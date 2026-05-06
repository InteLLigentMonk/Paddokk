import { queryOptions, useInfiniteQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getJourneyDetailFn,
  getJourneyPostsFn,
  getPostCommentsFn,
  createCommentFn,
  deleteCommentFn,
  replyToCommentFn,
} from "@/lib/api/journey-detail.server";

const POSTS_PAGE_SIZE = 20;

export const journeyDetailQueryOptions = (journeyId: number) =>
  queryOptions({
    queryKey: ["journey-detail", journeyId],
    queryFn: () => getJourneyDetailFn({ data: { journeyId } }),
  });

export const postCommentsQueryOptions = (postId: number) =>
  queryOptions({
    queryKey: ["post-comments", postId],
    queryFn: () => getPostCommentsFn({ data: { postId, page: 1, pageSize: 50 } }),
  });

export function useJourneyPostsInfinite(journeyId: number) {
  return useInfiniteQuery({
    queryKey: ["journey-posts", journeyId],
    queryFn: ({ pageParam = 0 }) =>
      getJourneyPostsFn({ data: { journeyId, skip: pageParam, take: POSTS_PAGE_SIZE } }),
    initialPageParam: 0,
    getNextPageParam: (lastPage, allPages) => {
      if (!lastPage || lastPage.length < POSTS_PAGE_SIZE) return undefined;
      return allPages.reduce((acc, page) => acc + page.length, 0);
    },
  });
}

export function useCreateComment(postId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (content: string) => createCommentFn({ data: { postId, content } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["post-comments", postId] });
    },
  });
}

export function useDeleteComment(postId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (commentId: number) => deleteCommentFn({ data: { commentId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["post-comments", postId] });
    },
  });
}

export function useReplyToComment(postId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ content, parentCommentId }: { content: string; parentCommentId: number }) =>
      replyToCommentFn({ data: { postId, content, parentCommentId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["post-comments", postId] });
    },
  });
}
