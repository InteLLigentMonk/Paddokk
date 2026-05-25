import { createServerFn } from "@tanstack/react-start";
import { CommentsDeleteCommentParams } from "@/generated/api-zod/comments/comments.zod";
import {
  JourneysCreateJourneyPostBody,
  JourneysCreateJourneyPostParams,
  JourneysGetJourneyParams,
  JourneysGetJourneyPostsParams,
  JourneysGetJourneyPostsQueryParams,
} from "@/generated/api-zod/journeys/journeys.zod";
import {
  PostCommentsCreateCommentBody,
  PostCommentsCreateCommentParams,
  PostCommentsGetPostCommentsParams,
  PostCommentsGetPostCommentsQueryParams,
} from "@/generated/api-zod/post-comments/post-comments.zod";
import { commentsDeleteComment } from "@/generated/api/comments/comments";
import {
  journeysCreateJourneyPost,
  journeysGetJourney,
  journeysGetJourneyPosts,
} from "@/generated/api/journeys/journeys";
import {
  postCommentsCreateComment,
  postCommentsGetPostComments,
} from "@/generated/api/post-comments/post-comments";

const journeyPostsSchema = JourneysGetJourneyPostsParams.extend(
  JourneysGetJourneyPostsQueryParams.shape,
);

const postCommentsSchema = PostCommentsGetPostCommentsParams.extend(
  PostCommentsGetPostCommentsQueryParams.shape,
);

const createCommentSchema = PostCommentsCreateCommentParams.extend(
  PostCommentsCreateCommentBody.shape,
);

const createJourneyPostSchema = JourneysCreateJourneyPostParams.extend(
  JourneysCreateJourneyPostBody.shape,
);

export const getJourneyDetailFn = createServerFn({ method: "GET" })
  .inputValidator(JourneysGetJourneyParams)
  .handler(
    async ({ data: { journeyId } }) => await journeysGetJourney(journeyId),
  );

export const getJourneyPostsFn = createServerFn({ method: "GET" })
  .inputValidator(journeyPostsSchema)
  .handler(
    async ({ data: { journeyId, skip, take } }) =>
      await journeysGetJourneyPosts(journeyId, { skip, take }),
  );

export const getPostCommentsFn = createServerFn({ method: "GET" })
  .inputValidator(postCommentsSchema)
  .handler(
    async ({ data: { postId, page, pageSize } }) =>
      await postCommentsGetPostComments(postId, { page, pageSize }),
  );

export const createCommentFn = createServerFn({ method: "POST" })
  .inputValidator(createCommentSchema)
  .handler(
    async ({ data: { postId, ...body } }) =>
      await postCommentsCreateComment(postId, { postId, ...body }),
  );

export const deleteCommentFn = createServerFn({ method: "POST" })
  .inputValidator(CommentsDeleteCommentParams)
  .handler(async ({ data: { commentId } }) => {
    await commentsDeleteComment(commentId);
  });

export const replyToCommentFn = createServerFn({ method: "POST" })
  .inputValidator(createCommentSchema)
  .handler(
    async ({ data: { postId, ...body } }) =>
      await postCommentsCreateComment(postId, { postId, ...body }),
  );

export const createJourneyPostFn = createServerFn({ method: "POST" })
  .inputValidator(createJourneyPostSchema)
  .handler(
    async ({ data: { journeyId, ...body } }) =>
      await journeysCreateJourneyPost(journeyId, { journeyId, ...body }),
  );
