import { createServerFn } from "@tanstack/react-start";
import {
  UserJourneysCreateJourneyBody,
  UserJourneysDeleteJourneyParams,
  UserJourneysSetDefaultActiveJourneyParams,
  UserJourneysUpdateJourneyBody,
  UserJourneysUpdateJourneyParams,
} from "@/generated/api-zod/user-journeys/user-journeys.zod";
import {
  userJourneysCreateJourney,
  userJourneysDeleteJourney,
  userJourneysGetDefaultActiveJourney,
  userJourneysGetUserJourneys,
  userJourneysSetDefaultActiveJourney,
  userJourneysUpdateJourney,
} from "@/generated/api/user-journeys/user-journeys";

// UpdateJourneyCommand is a full-replace shape on the wire, but the UI does
// per-section partial edits. Body fields are optional; journeyId from Params is
// re-applied on top so it stays required.
const updateJourneySchema = UserJourneysUpdateJourneyBody.partial().extend(
  UserJourneysUpdateJourneyParams.shape,
);

export const createJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(UserJourneysCreateJourneyBody)
  .handler(async ({ data }) => await userJourneysCreateJourney(data));

export const getUserJourneysFn = createServerFn({ method: "GET" }).handler(
  async () => await userJourneysGetUserJourneys(),
);

// Returns null when the backend has no default-active journey (404 surfaces as ApiError).
export const getDefaultActiveJourneyFn = createServerFn({
  method: "GET",
}).handler(async () => {
  try {
    return await userJourneysGetDefaultActiveJourney();
  } catch {
    return null;
  }
});

export const deleteUserJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(UserJourneysDeleteJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await userJourneysDeleteJourney(journeyId);
  });

export const updateJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(updateJourneySchema)
  .handler(
    async ({ data }) =>
      await userJourneysUpdateJourney(data.journeyId, {
        journeyId: data.journeyId,
        title: data.title ?? null,
        description: data.description ?? null,
        category: data.category ?? null,
        status: data.status ?? null,
        completedAt: data.completedAt ?? null,
        targetCompletedAt: data.targetCompletedAt ?? null,
        coverImageUrl: data.coverImageUrl ?? null,
        isPublic: data.isPublic ?? null,
      }),
  );

export const setDefaultActiveJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(UserJourneysSetDefaultActiveJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await userJourneysSetDefaultActiveJourney(journeyId);
  });
