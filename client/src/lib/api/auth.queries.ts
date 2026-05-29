import { queryOptions } from "@tanstack/react-query";
import { getAuthSession } from "@/lib/auth-session";
import { authKeys } from "@/lib/api/auth.keys";

export const authSessionQueryOptions = () =>
  queryOptions({
    queryKey: authKeys.session,
    queryFn: () => getAuthSession(),
    staleTime: 5 * 60 * 1000,
  });
