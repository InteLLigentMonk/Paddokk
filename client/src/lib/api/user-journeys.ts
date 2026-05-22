import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import type { CreateJourneyCommand, JourneyDto } from "@/generated/api/schemas";
import {
  userJourneysCreateJourney,
  userJourneysDeleteJourney,
  userJourneysGetDefaultActiveJourney,
  userJourneysGetUserJourneys,
  userJourneysSetDefaultActiveJourney,
  userJourneysUpdateJourney,
} from "@/generated/api/user-journeys/user-journeys";

const journeyIdSchema = z.object({ journeyId: z.coerce.number() });

const createJourneySchema = z.object({
  title: z.string().min(3).max(200),
  description: z.string().nullable().optional(),
  category: z.number(),
  userCarId: z.coerce.number(),
  setAsDefaultActive: z.boolean().optional(),
  targetCompletedAt: z.string().nullable().optional(),
});

export const createJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(createJourneySchema)
  .handler(async ({ data }) => {
    const result = await userJourneysCreateJourney(
      data as CreateJourneyCommand,
    );
    return result.data as JourneyDto;
  });

const updateJourneySchema = z.object({
  journeyId: z.coerce.number(),
  title: z.string().nullable().optional(),
  description: z.string().nullable().optional(),
  category: z.number().nullable().optional(),
  status: z.number().nullable().optional(),
  completedAt: z.string().nullable().optional(),
  targetCompletedAt: z.string().nullable().optional(),
  coverImageUrl: z.string().nullable().optional(),
  isPublic: z.boolean().nullable().optional(),
});

export const getUserJourneysFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await userJourneysGetUserJourneys();
    return result.data as Array<JourneyDto>;
  },
);

export const getDefaultActiveJourneyFn = createServerFn({
  method: "GET",
}).handler(async () => {
  try {
    const result = await userJourneysGetDefaultActiveJourney();
    return result.status === 200 ? result.data : null;
  } catch {
    return null;
  }
});

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
      isPublic: fields.isPublic ?? null,
    });
    return result.data as JourneyDto;
  });

export const setDefaultActiveJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await userJourneysSetDefaultActiveJourney(journeyId);
  });
