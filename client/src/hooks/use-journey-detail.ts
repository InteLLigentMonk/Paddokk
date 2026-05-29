import {
  queryOptions,
  useInfiniteQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  createCommentFn,
  createJourneyPostFn,
  deleteCommentFn,
  getJourneyDetailFn,
  getJourneyPostsFn,
  getPostCommentsFn,
  replyToCommentFn,
  reportCommentFn,
} from "@/lib/api/journey-detail";
import { journeyKeys } from "@/lib/api/journeys.keys";
import { requirePage } from "@/lib/api/infinite";
import { notify } from "@/integrations/mantine/use-notifications";

const POSTS_PAGE_SIZE = 20;
const COMMENTS_PAGE_SIZE = 20;

export const journeyDetailQueryOptions = (journeyId: number) =>
  queryOptions({
    queryKey: journeyKeys.detail(journeyId),
    queryFn: () => getJourneyDetailFn({ data: { journeyId } }),
  });

export const postCommentsQueryOptions = (postId: number) =>
  queryOptions({
    queryKey: journeyKeys.postComments(postId),
    queryFn: () =>
      getPostCommentsFn({
        data: { postId, page: 1, pageSize: COMMENTS_PAGE_SIZE },
      }),
  });

export function useJourneyPostsInfinite(journeyId: number) {
  return useInfiniteQuery({
    queryKey: journeyKeys.posts(journeyId),
    queryFn: ({ pageParam = 1 }) =>
      requirePage(
        getJourneyPostsFn({
          data: { journeyId, page: pageParam, pageSize: POSTS_PAGE_SIZE },
        }),
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function useCreateComment(postId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (content: string) =>
      createCommentFn({ data: { postId, content } }),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: journeyKeys.postComments(postId),
      });
    },
  });
}

export function useDeleteComment(postId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (commentId: number) => deleteCommentFn({ data: { commentId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: journeyKeys.postComments(postId),
      });
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
      queryClient.invalidateQueries({
        queryKey: journeyKeys.postComments(postId),
      });
    },
  });
}

export function useReportComment() {
  return useMutation({
    mutationFn: ({
      commentId,
      reason,
    }: {
      commentId: number;
      reason: string;
    }) => reportCommentFn({ data: { commentId, reason } }),
    onSuccess: (result) => {
      if (result.kind === "notImplemented") {
        notify.info({
          title: result.title,
          message:
            result.message ||
            "Moderation is not implemented yet. The comment has not been reported, but we appreciate you taking the time to provide feedback.",
          autoClose: 6000,
        });
        return;
      }
      notify.success({ message: "Thank you - The comment has been reported." });
    },
    onError: (error) => {
      notify.error({
        message:
          error instanceof Error
            ? error.message
            : "Could not report the comment.",
      });
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
      images: Array<PostImagePayload>;
    }) => createJourneyPostFn({ data: { journeyId, ...payload } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: journeyKeys.posts(journeyId) });
    },
  });
}
