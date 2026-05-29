import { ApiError } from "./api-error";

/**
 * Guards an infinite-query page fetch against an empty-body response.
 *
 * `apiFetcher` returns `undefined` for an HTTP 204 (no content), which is
 * correct for void endpoints but poisonous for a paged data endpoint: the
 * `undefined` gets cached as a page, then crashes `getNextPageParam` and every
 * `data.pages.flatMap(...)` consumer. Throwing here turns a transient empty
 * response (e.g. a cold-start race) into a retryable error so React Query's
 * `retry` self-heals instead of caching a broken page.
 */
export async function requirePage<T>(
  page: Promise<T | null | undefined>,
): Promise<T> {
  const result = await page;
  if (result == null) {
    throw new ApiError(502, "Received an empty response while loading a page");
  }
  return result;
}
