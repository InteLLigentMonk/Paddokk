import postgres from "postgres";

const connectionString = process.env.BETTER_AUTH_DB_CONNECTION_STRING;

if (!connectionString) {
  throw new Error(
    "BETTER_AUTH_DB_CONNECTION_STRING environment variable is required",
  );
}

const sql = postgres(connectionString);

async function resetDatabase() {
  console.log("Dropping all existing tables...");

  try {
    // Drop all tables by dropping the public schema and recreating it
    await sql`DROP SCHEMA public CASCADE`;
    await sql`CREATE SCHEMA public`;
    await sql`GRANT ALL ON SCHEMA public TO public`;

    console.log("✓ All tables dropped successfully");
  } catch (error) {
    console.error("Error dropping tables:", error);
    throw error;
  } finally {
    await sql.end();
  }
}

resetDatabase();
