import { createServerFn } from "@tanstack/react-start";
import {
  limitsGetCarLimits,
  limitsGetImageLimits,
} from "@/generated/api/limits/limits";

export const getCarLimitFn = createServerFn({ method: "GET" }).handler(
  async () => await limitsGetCarLimits(),
);

export const getImageLimitsFn = createServerFn({ method: "GET" }).handler(
  async () => await limitsGetImageLimits(),
);
