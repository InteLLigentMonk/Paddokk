import { ApiError } from './api-error'

export type ErrorType<_TError> = ApiError
export type BodyType<TData> = TData

const apiUrl = import.meta.env.VITE_API_URL
if (!apiUrl) throw new Error('VITE_API_URL is required')

const getAuthToken = async (): Promise<string | null> => {
  if (typeof window === 'undefined') {
    // Server: call auth.api.token() — same pattern as auth.api.getSession()
    try {
      const [{ getRequestHeaders }, { auth }] = await Promise.all([
        import('@tanstack/react-start/server'),
        import('@/lib/auth'),
      ])
      // auth.api.token is added dynamically by the JWT plugin — cast to access it
      type ApiWithToken = typeof auth.api & {
        token: (opts: { headers: Headers }) => Promise<{ token: string }>
      }
      const result = await (auth.api as ApiWithToken).token({ headers: getRequestHeaders() })
      return result?.token ?? null
    } catch {
      return null
    }
  }

  // Client: use Better Auth client JWT plugin
  try {
    const { authClient } = await import('@/lib/auth-client')
    const { data } = await authClient.token()
    return data?.token ?? null
  } catch {
    return null
  }
}

const normalizeHeaders = (incoming: HeadersInit): Record<string, string> => {
  if (incoming instanceof Headers) return Object.fromEntries(incoming)
  if (Array.isArray(incoming)) return Object.fromEntries(incoming)
  return incoming as Record<string, string>
}

export const apiFetcher = async <T>(url: string, options?: RequestInit): Promise<T> => {
  const token = await getAuthToken()

  const headers: Record<string, string> = { 'Content-Type': 'application/json' }

  if (token) headers['Authorization'] = `Bearer ${token}`

  if (options?.headers) {
    Object.assign(headers, normalizeHeaders(options.headers))
  }

  const res = await fetch(`${apiUrl}${url}`, { ...options, headers })

  if (!res.ok) {
    const problem = await res.json().catch(() => null)
    throw new ApiError(
      res.status,
      problem?.title ?? `API error: ${res.status}`,
      problem?.detail,
      problem?.errors,
    )
  }

  if (res.status === 204) {
    return { data: null, status: res.status, headers: res.headers } as T
  }

  const data = await res.json()
  return { data, status: res.status, headers: res.headers } as T
}
