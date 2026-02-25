import { defineConfig } from "orval";

export default defineConfig({
  paddokkApi: {
    input: {
      target: "./swagger.json", // Use local file for reliability
    },
    output: {
      mode: "tags-split", // Split by controller tags
      target: "src/generated/api/",
      schemas: "src/generated/api/api.schemas.ts",
      client: "fetch",
      mock: false,
      override: {
        zod: {
          strict: {
            param: true,
            query: true,
            body: true,
            response: true,
          },
          generate: {
            param: true,
            query: true,
            body: true,
            response: true,
          },
        },
        fetch: {
          includeHttpResponseReturnType: true,
        },
        mutator: {
          path: "src/lib/api/client.ts",
          name: "apiFetcher",
        },
      },
    },
    hooks: {
      afterAllFilesWrite: "prettier --write",
    },
  },
});
