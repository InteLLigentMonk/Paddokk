import { queryOptions } from "@tanstack/react-query";
import { getAuthSession } from "@/lib/auth-session.server";

export const authSessionQueryOptions = () =>
  queryOptions({
    queryKey: ["auth-session"],
    queryFn: () => getAuthSession(),
    staleTime: 5 * 60 * 1000,
  });
