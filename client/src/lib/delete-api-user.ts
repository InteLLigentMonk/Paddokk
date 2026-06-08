import { APIError } from "better-auth/api";

interface DeleteApiUserParams {
  apiUrl: string;
  token: string | null;
  fetchImpl?: typeof fetch;
}

/**
 * Anonymises the user on the .NET API as part of account deletion.
 *
 * Throws an APIError on any failure so that BetterAuth's `beforeDelete` hook
 * aborts and does NOT hard-delete its own records. Otherwise a failed API call
 * would leave the API-side PII (email, name, avatar) orphaned with no auth
 * record pointing to it and no way for the user to retry deletion.
 */
export async function deleteApiUser({
  apiUrl,
  token,
  fetchImpl = fetch,
}: DeleteApiUserParams): Promise<void> {
  if (!token) {
    throw new APIError("INTERNAL_SERVER_ERROR", {
      message: "Could not delete account: failed to obtain API token",
    });
  }

  const response = await fetchImpl(
    `${apiUrl.replace(/\/$/, "")}/api/v1/Users/me`,
    {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    },
  );

  if (!response.ok) {
    throw new APIError("INTERNAL_SERVER_ERROR", {
      message: `Could not delete account: API returned ${response.status}`,
    });
  }
}
