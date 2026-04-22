---
name: new-bff-route
description: Backend For Frontend expert. Always invoke when creating a new BFF server function in TanStack Start that connects the frontend to the .NET API via Orval-generated clients. Do not attempt to implement server functions directly - use this skill first.
model: sonnet
triggers:
  - TRIGGER AUTOMATICALLY when connecting frontend to any .NET endpoint via a server function
  - TRIGGER AUTOMATICALLY when the user asks to add a server function, BFF route, or hook that calls the API
  - DO NOT call the .NET API directly from components — always go through a server function first
---

# New BFF Route

When connecting frontend to a .NET API endpoint through the BFF layer.

## Prerequisites
- .NET endpoint exists and is running
- Orval client has been regenerated (`npx orval`)
- Generated client functions are available in the Orval output directory

## 1. Server Function (BFF)
Location: `app/server-functions/{feature-name}.ts`

```typescript
import { createServerFn } from '@tanstack/start'
import { generatedApiFunction } from '../generated-api-client'

export const getEntity = createServerFn({ method: 'GET' })
  .validator((input: { id: string }) => z.object({ id: z.string().uuid() }).parse(input))
  .handler(async ({ data }) => {
    const result = await generatedApiFunction(data.id)
    return result
  })

export const createEntity = createServerFn({ method: 'POST' })
  .validator((input: CreateEntityInput) => createEntitySchema.parse(input))
  .handler(async ({ data }) => {
    const result = await generatedCreateFunction(data)
    return result
  })
```

Key points:
- Always validate input with Zod in the `.validator()` step
- The server function calls the Orval-generated client, never raw fetch
- Handle auth token forwarding if the .NET endpoint requires authentication
- Keep server functions thin — transform data if needed, but no business logic

## 2. TanStack Query Hook
Location: `app/hooks/use-{feature-name}.ts`

```typescript
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query'
import { getEntity, createEntity } from '../server-functions/{feature-name}'

export const entityQueryOptions = (id: string) =>
  queryOptions({
    queryKey: ['{feature-name}', id],
    queryFn: () => getEntity({ data: { id } }),
  })

export function useCreateEntity() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateEntityInput) => createEntity({ data }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['{feature-name}'] })
    },
  })
}
```

## 3. Component Integration
- Use `useSuspenseQuery(entityQueryOptions(id))` for reads
- Use `useCreateEntity()` hook for mutations
- Forms use TanStack Form with Mantine components
- Show loading/error states via Mantine's Skeleton/Alert components

## 4. Route Loader (if needed)
If the data should be fetched before the route renders:

```typescript
export const Route = createFileRoute('/{feature-name}/$id')({
  loader: ({ params }) => entityQueryOptions(params.id),
  component: EntityPage,
})
```

## Checklist
- [ ] Server function created with Zod validation
- [ ] Server function uses Orval client (not raw fetch)
- [ ] Auth token forwarded if endpoint requires auth
- [ ] TanStack Query hook with proper query keys
- [ ] Mutation invalidates relevant queries on success
- [ ] Component uses Mantine UI components
- [ ] Route loader set up if data is needed before render
