import { defineConfig } from 'orval'

export default defineConfig({
  paddokkApi: {
    input: {
      target: './swagger.json', // Use local file for reliability
    },
    output: {
      mode: 'tags-split', // Split by controller tags
      target: 'src/generated/api/api.ts',
      schemas: 'src/generated/api/api.schemas.ts',
      client: 'react-query',
      mock: false,
      override: {
        mutator: {
          path: 'src/lib/api/client.ts',
          name: 'apiFetcher',
        },
      },
    },
    hooks: {
      afterAllFilesWrite: 'prettier --write',
    },
  },
})
