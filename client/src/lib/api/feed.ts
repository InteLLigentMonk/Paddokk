import { createServerFn } from "@tanstack/react-start";
import { FeedGetFeedQueryParams } from "@/generated/api-zod/feed/feed.zod";
import { feedGetFeed } from "@/generated/api/feed/feed";

/**
 * The authenticated user's personalised, strictly chronological feed. The actor is
 * resolved server-side from the session — there are no actor params on the wire, so a
 * caller can only ever fetch their own feed.
 */
export const getFeedFn = createServerFn({ method: "GET" })
  .inputValidator(FeedGetFeedQueryParams)
  .handler(async ({ data }) => await feedGetFeed(data));
