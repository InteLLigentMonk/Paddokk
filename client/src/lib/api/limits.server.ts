import { limitsGetCarLimits } from "@/generated/api/limits/limits";
import { CarLimitDto } from "@/generated/api/schemas";
import { createServerFn } from "@tanstack/react-start";

export const getCarLimitFn = createServerFn({ method: "GET" }).handler(
  async () => {
    const result = await limitsGetCarLimits();
    return result.data as CarLimitDto;
  },
);
