import { limitsGetCarLimits, limitsGetImageLimits } from "@/generated/api/limits/limits";
import { CarLimitDto, ImageLimitsDto } from "@/generated/api/schemas";
import { createServerFn } from "@tanstack/react-start";

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
