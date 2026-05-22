import { createServerFn } from "@tanstack/react-start";
import type { CarLimitDto, ImageLimitsDto } from "@/generated/api/schemas";
import {
  limitsGetCarLimits,
  limitsGetImageLimits,
} from "@/generated/api/limits/limits";

export const getCarLimitFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await limitsGetCarLimits();
    return result.data as CarLimitDto;
  },
);

export const getImageLimitsFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await limitsGetImageLimits();
    return result.data as ImageLimitsDto;
  },
);
