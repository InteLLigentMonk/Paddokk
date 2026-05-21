import {
  queryOptions,
  useInfiniteQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  getJourneyDetailFn,
  getJourneyPostsFn,
  getPostCommentsFn,
  createCommentFn,
  deleteCommentFn,
  replyToCommentFn,
  createJourneyPostFn,
} from "@/lib/api/journey-detail";

const POSTS_PAGE_SIZE = 20;

export const journeyDetailQueryOptions = (journeyId: number) =>
  queryOptions({
    queryKey: ["journey-detail", journeyId],
    queryFn: () => getJourneyDetailFn({ data: { journeyId } }),
  });

export const postCommentsQueryOptions = (postId: number) =>
  queryOptions({
    queryKey: ["post-comments", postId],
    queryFn: () =>
      getPostCommentsFn({ data: { postId, page: 1, pageSize: 50 } }),
  });

export function useJourneyPostsInfinite(journeyId: number) {
  return useInfiniteQuery({
    queryKey: ["journey-posts", journeyId],
    queryFn: ({ pageParam = 0 }) =>
      getJourneyPostsFn({
        data: { journeyId, skip: pageParam, take: POSTS_PAGE_SIZE },
      }),
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
    mutationFn: (content: string) =>
      createCommentFn({ data: { postId, content } }),
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
    mutationFn: ({
      content,
      parentCommentId,
    }: {
      content: string;
      parentCommentId: number;
    }) => replyToCommentFn({ data: { postId, content, parentCommentId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["post-comments", postId] });
    },
  });
}

interface PostImagePayload {
  imageUrl: string;
  caption?: string | null;
  sortOrder?: number;
}

export function useCreateJourneyPost(journeyId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: {
      textContent: string | null;
      images: PostImagePayload[];
    }) => createJourneyPostFn({ data: { journeyId, ...payload } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["journey-posts", journeyId] });
    },
  });
}
