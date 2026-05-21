import { createIsomorphicFn } from "@tanstack/react-start";
import { ApiError } from "./api-error";

export type ErrorType = ApiError;
export type BodyType<TData> = TData;

const apiUrl = import.meta.env.VITE_API_URL;
if (!apiUrl) throw new Error("VITE_API_URL is required");

const getAuthToken = createIsomorphicFn()
  .server(async (): Promise<string | null> => {
    try {
      const { getRequestHeaders } = await import(
        "@tanstack/react-start/server"
      );
      const { auth } = await import("@/lib/auth.server");
      const baseUrl = process.env.BETTER_AUTH_URL ?? "http://localhost:3000";
      const res = await auth.handler(
        new Request(`${baseUrl}/api/auth/token`, {
          headers: getRequestHeaders(),
        }),
      );
      if (!res.ok) return null;
      const { token } = await res.json();
      return token ?? null;
    } catch {
      return null;
    }
  })
  .client(async (): Promise<string | null> => {
    try {
      const { authClient } = await import("@/lib/auth-client");
      const { data } = await authClient.token();
      return data?.token ?? null;
    } catch {
      return null;
    }
  });

const normalizeHeaders = (incoming: HeadersInit): Record<string, string> => {
  if (incoming instanceof Headers) return Object.fromEntries(incoming);
  if (Array.isArray(incoming)) return Object.fromEntries(incoming);
  return incoming;
};

export const apiFetcher = async <T>(
  url: string,
  options?: RequestInit,
): Promise<T> => {
  const token = await getAuthToken();
  const isFormData = options?.body instanceof FormData;
  const headers: Record<string, string> = isFormData
    ? {}
    : { "Content-Type": "application/json" };

  if (token) headers["Authorization"] = `Bearer ${token}`;

  if (options?.headers) {
    Object.assign(headers, normalizeHeaders(options.headers));
  }

  const res = await fetch(`${apiUrl}${url}`, { ...options, headers });

  if (!res.ok) {
    const problem = await res.json().catch(() => null);
    throw new ApiError(
      res.status,
      problem?.title ?? `API error: ${res.status}`,
      problem?.detail,
      problem?.errors,
    );
  }

  if (res.status === 204) {
    return { data: null, status: res.status, headers: res.headers } as T;
  }

  const data = await res.json();
  return { data, status: res.status, headers: res.headers } as T;
};
