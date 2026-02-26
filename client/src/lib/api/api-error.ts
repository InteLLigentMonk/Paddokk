export class ApiError extends Error {
  status: number
  detail?: string
  errors?: Record<string, string[]>

  constructor(
    status: number,
    title: string,
    detail?: string,
    errors?: Record<string, string[]>,
  ) {
    super(title)
    this.status = status
    this.detail = detail
    this.errors = errors
  }
}
