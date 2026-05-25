import { defineConfig } from "orval";

export default defineConfig({
  paddokkApi: {
    input: {
      target: "./swagger.json",
    },
    output: {
      mode: "tags-split",
      target: "./src/generated/api",
      schemas: "./src/generated/api/schemas",
      client: "fetch",
      mock: false,
      override: {
        fetch: {
          includeHttpResponseReturnType: false,
        },
        mutator: {
          path: "./src/lib/api/client.ts",
          name: "apiFetcher",
        },
      },
    },
    hooks: {
      afterAllFilesWrite: "prettier --write",
    },
  },
  paddokkApiZod: {
    input: {
      target: "./swagger.json",
    },
    output: {
      mode: "tags-split",
      target: "./src/generated/api-zod",
      client: "zod",
      fileExtension: ".zod.ts",
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
      },
    },
    hooks: {
      afterAllFilesWrite: "prettier --write",
    },
  },
});
