import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import type {
  CommentsPagedResponse,
  JourneyDto,
  JourneyPostDto,
} from "@/generated/api/schemas";
import {
  journeysCreateJourneyPost,
  journeysGetJourney,
  journeysGetJourneyPosts,
} from "@/generated/api/journeys/journeys";
import {
  postCommentsCreateComment,
  postCommentsGetPostComments,
} from "@/generated/api/post-comments/post-comments";
import { commentsDeleteComment } from "@/generated/api/comments/comments";

const journeyIdSchema = z.object({ journeyId: z.coerce.number() });

const journeyPostsSchema = z.object({
  journeyId: z.coerce.number(),
  skip: z.coerce.number().min(0).default(0),
  take: z.coerce.number().min(1).max(50).default(20),
});

const postCommentsSchema = z.object({
  postId: z.coerce.number(),
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(50).default(20),
});

const createCommentSchema = z.object({
  postId: z.coerce.number(),
  content: z.string().min(1).max(500),
  parentCommentId: z.coerce.number().optional(),
});

const deleteCommentSchema = z.object({
  commentId: z.coerce.number(),
});

export const getJourneyDetailFn = createServerFn({ method: "GET" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    const result = await journeysGetJourney(journeyId);
    return result.data as JourneyDto;
  });

export const getJourneyPostsFn = createServerFn({ method: "GET" })
  .inputValidator(journeyPostsSchema)
  .handler(async ({ data: { journeyId, skip, take } }) => {
    const result = await journeysGetJourneyPosts(journeyId, { skip, take });
    return result.data as Array<JourneyPostDto>;
  });

export const getPostCommentsFn = createServerFn({ method: "GET" })
  .inputValidator(postCommentsSchema)
  .handler(async ({ data: { postId, page, pageSize } }) => {
    const result = await postCommentsGetPostComments(postId, {
      page,
      pageSize,
    });
    return result.data as CommentsPagedResponse;
  });

export const createCommentFn = createServerFn({ method: "POST" })
  .inputValidator(createCommentSchema)
  .handler(async ({ data: { postId, content, parentCommentId } }) => {
    const result = await postCommentsCreateComment(postId, {
      postId,
      content,
      parentCommentId: parentCommentId ?? null,
    });
    return result.data;
  });

export const deleteCommentFn = createServerFn({ method: "POST" })
  .inputValidator(deleteCommentSchema)
  .handler(async ({ data: { commentId } }) => {
    await commentsDeleteComment(commentId);
  });

export const replyToCommentFn = createServerFn({ method: "POST" })
  .inputValidator(createCommentSchema)
  .handler(async ({ data: { postId, content, parentCommentId } }) => {
    const result = await postCommentsCreateComment(postId, {
      postId,
      content,
      parentCommentId: parentCommentId ?? null,
    });
    return result.data;
  });

const createPostSchema = z.object({
  journeyId: z.coerce.number(),
  textContent: z.string().nullable(),
  images: z.array(
    z.object({
      imageUrl: z.string(),
      caption: z.string().nullable().optional(),
      sortOrder: z.number().optional(),
    }),
  ),
});

export const createJourneyPostFn = createServerFn({ method: "POST" })
  .inputValidator(createPostSchema)
  .handler(async ({ data: { journeyId, textContent, images } }) => {
    const result = await journeysCreateJourneyPost(journeyId, {
      journeyId,
      textContent,
      images,
    });
    return result.data as JourneyPostDto;
  });
