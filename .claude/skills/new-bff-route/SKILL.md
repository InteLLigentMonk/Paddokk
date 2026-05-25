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
- Orval has been regenerated: `cd client && pnpm orval` (generates both fetch SDK and Zod schemas)
- Generated SDK in `client/src/generated/api/<tag>/<tag>.ts`
- Generated Zod schemas in `client/src/generated/api-zod/<tag>/<tag>.zod.ts`

## Type Safety Rules (strict — no exceptions)

1. **Always import the generated SDK function** from `@/generated/api/<tag>/<tag>`. Never call `apiFetcher` directly with handwritten URLs.
2. **Always use generated Zod schemas as `inputValidator`** for inputs that map to a backend DTO. Handwritten Zod is only allowed for inputs with no backend counterpart.
3. **Never write `as X` casts** in any file under `client/src/lib/api/*.ts`. The only acceptable casts are the two `as T` already present in [client/src/lib/api/client.ts](client/src/lib/api/client.ts).
4. **Return the SDK result directly**. No `.data` unwrap, no manual reshape. The SDK returns the DTO/response type as-is.

## 1. Server Function (BFF)

Location: `client/src/lib/api/<feature>.ts`

### Read endpoint

```typescript
import { createServerFn } from "@tanstack/react-start";
import { UserCarsGetUserCarParams } from "@/generated/api-zod/user-cars/user-cars.zod";
import { userCarsGetUserCar } from "@/generated/api/user-cars/user-cars";

export const getUserCarFn = createServerFn({ method: "GET" })
  .inputValidator(UserCarsGetUserCarParams)
  .handler(async ({ data: { carId } }) => await userCarsGetUserCar(carId));
```

### Create endpoint (body-only)

```typescript
import { UserCarsCreateUserCarBody } from "@/generated/api-zod/user-cars/user-cars.zod";
import { userCarsCreateUserCar } from "@/generated/api/user-cars/user-cars";

export const createUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsCreateUserCarBody)
  .handler(async ({ data }) => await userCarsCreateUserCar(data));
```

### Update endpoint with full-replace shape (path + body)

```typescript
const updateSchema = UserCarsUpdateUserCarParams.extend(
  UserCarsUpdateUserCarBody.shape,
);

export const updateUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(updateSchema)
  .handler(async ({ data }) =>
    await userCarsUpdateUserCar(data.carId, data),
  );
```

### Update endpoint with partial-patch UX (UI sends only changed fields)

When the backend command has all fields required (full-replace shape on the wire) but the UI does per-section edits, use `Body.partial()` and null-fill in the handler. **Put `.partial()` first, then `.extend(Params.shape)` so the path key stays required.**

```typescript
const updateUserCarSchema = UserCarsUpdateUserCarBody.partial().extend(
  UserCarsUpdateUserCarParams.shape,
);

export const updateUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(updateUserCarSchema)
  .handler(async ({ data }) =>
    await userCarsUpdateUserCar(data.carId, {
      carId: data.carId,
      customBuildName: data.customBuildName ?? null,
      nickname: data.nickname ?? null,
      // …null-fill the remaining body fields
    }),
  );
```

### Delete / void endpoint (204 response)

```typescript
import { UserCarsDeleteUserCarParams } from "@/generated/api-zod/user-cars/user-cars.zod";

export const deleteUserCarFn = createServerFn({ method: "POST" })
  .inputValidator(UserCarsDeleteUserCarParams)
  .handler(async ({ data: { carId } }) => {
    await userCarsDeleteUserCar(carId);
  });
```

## 2. Don'ts (anti-patterns from the old codebase)

```typescript
// DON'T: handwritten Zod that duplicates the DTO shape
const createUserCarSchema = z.object({
  isCustomBuild: z.boolean(),
  customBuildName: z.string().nullable().optional(),
  // …
});

// DON'T: handwritten URL via apiFetcher
const result = await apiFetcher<{ data: UserCarDto }>(`/api/v1/cars/${carId}`);
return result.data as UserCarDto;

// DON'T: cast on SDK return
const result = await userCarsGetUserCar(carId);
return result.data as UserCarDto;

// DON'T: cast on input
await userCarsCreateUserCar(data as CreateUserCarCommand);
```

## 3. TanStack Query Hook

Location: `client/src/lib/api/<feature>.queries.ts`

```typescript
import { queryOptions, useMutation, useQueryClient } from "@tanstack/react-query";
import { getUserCarFn, createUserCarFn } from "./user-cars";

export const userCarQueryOptions = (carId: number) =>
  queryOptions({
    queryKey: ["user-car", carId],
    queryFn: () => getUserCarFn({ data: { carId } }),
  });

export function useCreateUserCar() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createUserCarFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-cars"] });
    },
  });
}
```

## 4. Component Integration

- Reads: `useSuspenseQuery(userCarQueryOptions(carId))` — `query.data` is the DTO directly (no `.data` unwrap).
- Mutations: `useCreateUserCar().mutate({ data: { … } })`.
- Forms use TanStack Form with Mantine components.

## Checklist

- [ ] Server function imports SDK from `@/generated/api/…`
- [ ] `inputValidator` uses generated Zod from `@/generated/api-zod/…`
- [ ] No `apiFetcher` calls in feature file
- [ ] No `as X` casts (only the two in `client.ts` are allowed)
- [ ] Handler returns SDK result directly (no `.data` unwrap)
- [ ] Partial-update schemas use `Body.partial().extend(Params.shape)` ordering
- [ ] Query hook with proper query keys
- [ ] Mutation invalidates relevant queries on success
