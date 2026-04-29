import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import { userJourneysGetUserJourney } from "@/generated/api/user-journeys/user-journeys";
import { journeysGetJourneyPosts } from "@/generated/api/journeys/journeys";
import {
  postCommentsGetPostComments,
  postCommentsCreateComment,
} from "@/generated/api/post-comments/post-comments";
import type { JourneyDto, JourneyPostDto, CommentsPagedResponse } from "@/generated/api/schemas";

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
  content: z.string().min(1).max(2000),
});

export const getJourneyDetailFn = createServerFn({ method: "GET" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    const result = await userJourneysGetUserJourney(journeyId);
    return result.data as JourneyDto;
  });

export const getJourneyPostsFn = createServerFn({ method: "GET" })
  .inputValidator(journeyPostsSchema)
  .handler(async ({ data: { journeyId, skip, take } }) => {
    const result = await journeysGetJourneyPosts(journeyId, { skip, take });
    return result.data as JourneyPostDto[];
  });

export const getPostCommentsFn = createServerFn({ method: "GET" })
  .inputValidator(postCommentsSchema)
  .handler(async ({ data: { postId, page, pageSize } }) => {
    const result = await postCommentsGetPostComments(postId, { page, pageSize });
    return result.data as CommentsPagedResponse;
  });

export const createCommentFn = createServerFn({ method: "POST" })
  .inputValidator(createCommentSchema)
  .handler(async ({ data: { postId, content } }) => {
    const result = await postCommentsCreateComment(postId, { postId, content });
    return result.data;
  });
