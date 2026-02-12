import Axios from "axios";
import type { AxiosError, AxiosRequestConfig } from "axios";
import { authClient } from "@/lib/auth-client";

export type ErrorType<TError> = AxiosError<TError>;
export type BodyType<TData> = TData;

/**
 * Validate required environment variables
 */
const apiUrl = import.meta.env.VITE_API_URL;

if (!apiUrl) {
  throw new Error("VITE_API_URL environment variable is required");
}

/**
 * Custom Axios instance with Better Auth JWT integration
 */
const axiosInstance = Axios.create({
  baseURL: apiUrl,
  timeout: 30000, // 30 seconds (mobile-friendly)
  headers: {
    "Content-Type": "application/json",
  },
});

// Add request interceptor to inject JWT token
axiosInstance.interceptors.request.use(
  async (config) => {
    try {
      // Extract JWT token using Better Auth JWT plugin
      const tokenResponse = await authClient.token();
      const token = tokenResponse.data?.token;

      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      // Log token extraction failure but allow request to proceed
      // The API will return 401 if auth is required
      console.error("Failed to extract JWT token:", error);
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

// Add response interceptor to handle authentication errors
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Handle CORS errors
    if (!error.response && error.message === "Network Error") {
      console.error(
        "CORS Error: Ensure API has CORS configured for",
        import.meta.env.VITE_API_URL,
      );
    }

    // Handle authentication errors
    if (error.response?.status === 401) {
      // Try refreshing the token one more time before giving up
      const tokenResponse = await authClient.token();
      if (tokenResponse.data?.token) {
        // Retry the original request with the new token
        error.config.headers.Authorization = `Bearer ${tokenResponse.data.token}`;
        return axiosInstance(error.config);
      }
      // Refresh truly failed — session is dead
      await authClient.signOut();
      window.location.href = "/";
    }

    // Log errors in development
    if (import.meta.env.DEV) {
      console.error("API Error:", {
        url: error.config?.url,
        method: error.config?.method,
        status: error.response?.status,
        data: error.response?.data,
      });
    }

    return Promise.reject(error);
  },
);

/**
 * Custom Axios fetcher for Orval
 *
 * This fetcher:
 * - Accepts RequestInit (Fetch API signature) from Orval-generated code
 * - Converts it to AxiosRequestConfig with proper type safety
 * - Uses Axios with JWT token injection via interceptor
 * - Handles request cancellation via AbortSignal (TanStack Query)
 * - Works with SSR (token extracted server-side)
 */
export const apiFetcher = <T>(
  url: string,
  options?: RequestInit,
): Promise<T> => {
  // Convert headers safely to avoid type issues
  let headers: Record<string, string> = {};
  if (options?.headers) {
    if (options.headers instanceof Headers) {
      options.headers.forEach((value, key) => {
        headers[key] = value;
      });
    } else if (Array.isArray(options.headers)) {
      headers = Object.fromEntries(options.headers);
    } else {
      headers = options.headers;
    }
  }

  // Convert RequestInit to AxiosRequestConfig
  const axiosConfig: AxiosRequestConfig = {
    url,
    method: options?.method as string,
    data: options?.body ?? undefined, // Explicit undefined for no body
    headers,
    signal: options?.signal || undefined, // TanStack Query provides AbortSignal
  };

  return axiosInstance.request<T>(axiosConfig).then(({ data }) => data);
};
