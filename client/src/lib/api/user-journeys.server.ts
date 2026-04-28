import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import {
  userJourneysGetUserJourneys,
  userJourneysGetDefaultActiveJourney,
  userJourneysDeleteJourney,
  userJourneysUpdateJourney,
  userJourneysSetDefaultActiveJourney,
} from "@/generated/api/user-journeys/user-journeys";
import type { JourneyDto } from "@/generated/api/schemas";

const journeyIdSchema = z.object({ journeyId: z.coerce.number() });

const updateJourneySchema = z.object({
  journeyId: z.coerce.number(),
  title: z.string().nullable().optional(),
  description: z.string().nullable().optional(),
  category: z.number().nullable().optional(),
  status: z.number().nullable().optional(),
  completedAt: z.string().nullable().optional(),
  targetCompletedAt: z.string().nullable().optional(),
  coverImageUrl: z.string().nullable().optional(),
});

export const getUserJourneysFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await userJourneysGetUserJourneys();
    return result.data as JourneyDto[];
  },
);

export const getDefaultActiveJourneyFn = createServerFn({ method: "GET" }).handler(
  async () => {
    try {
      const result = await userJourneysGetDefaultActiveJourney();
      return result.status === 200 ? (result.data as JourneyDto) : null;
    } catch {
      return null;
    }
  },
);

export const deleteUserJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await userJourneysDeleteJourney(journeyId);
  });

export const updateJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(updateJourneySchema)
  .handler(async ({ data: { journeyId, ...fields } }) => {
    const result = await userJourneysUpdateJourney(journeyId, {
      journeyId,
      title: fields.title ?? null,
      description: fields.description ?? null,
      category: fields.category ?? null,
      status: fields.status ?? null,
      completedAt: fields.completedAt ?? null,
      targetCompletedAt: fields.targetCompletedAt ?? null,
      coverImageUrl: fields.coverImageUrl ?? null,
    });
    return result.data as JourneyDto;
  });

export const setDefaultActiveJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await userJourneysSetDefaultActiveJourney(journeyId);
  });
