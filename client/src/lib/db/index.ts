import { drizzle } from 'drizzle-orm/postgres-js'
import postgres from 'postgres'
import * as schema from './schema'

const connectionString = process.env.BETTER_AUTH_DB_CONNECTION_STRING

if (!connectionString) {
  throw new Error('BETTER_AUTH_DB_CONNECTION_STRING environment variable is required')
}

const client = postgres(connectionString)

export const db = drizzle(client, { schema })
