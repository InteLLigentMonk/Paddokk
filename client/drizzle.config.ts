import { defineConfig } from "drizzle-kit";

export default defineConfig({
  schema: "./src/lib/db/schema.ts",
  out: "./drizzle",
  dialect: "postgresql",
  strict: false,
  dbCredentials: {
    url: process.env.BETTER_AUTH_DB_CONNECTION_STRING!,
  },
});
