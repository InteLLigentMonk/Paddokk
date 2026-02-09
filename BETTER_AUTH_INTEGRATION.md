# Better-Auth Integration Guide

## Översikt

API:et har uppdaterats för att använda JWT-tokens från better-auth istället av egen autentiseringslogik. Better-auth hanterar nu all användarautentisering på frontend, medan API:et endast validerar JWT-tokens.

## Ändringar

### Borttagna filer:
- `API/Controllers/AuthController.cs` - All auth-logik flyttad till better-auth
- `API/Services/AuthService.cs` - Ersatt med UserService
- `API/Services/IAuthService.cs` - Ersatt med IUserService

### Nya filer:
- `API/Controllers/UsersController.cs` - Hanterar användarprofildata
- `API/Services/UserService.cs` - Service för användarhantering
- `API/Services/IUserService.cs` - Interface för UserService

### Uppdaterade filer:
- `API/Extensions/ServiceCollectionExtensions.cs` - JWT-konfiguration för better-auth
- `API/Extensions/ClaimsPricipalExtensions.cs` - Stöd för better-auth JWT claims
- `API/Controllers/DashboardController.cs` - Använder IUserService
- `API/appsettings.json` - Better-auth konfiguration

## Konfiguration

### 1. appsettings.json

Lägg till better-auth JWT-konfiguration i `appsettings.json`:

```json
{
  "BetterAuth": {
    "Jwt": {
      "SecretKey": "your-better-auth-secret-key-must-match-frontend-configuration"
    }
  }
}
```

**VIKTIGT:** SecretKey måste matcha den secret som används i better-auth konfigurationen på frontend!

### 2. Better-Auth Frontend Konfiguration

Din better-auth konfiguration på frontend bör inkludera samma secret:

```typescript
// auth.ts (frontend)
import { betterAuth } from "better-auth"

export const auth = betterAuth({
  secret: "your-better-auth-secret-key-must-match-frontend-configuration",
  database: {
    // din database config
  },
  // andra inställningar...
})
```

### 3. JWT Claims från Better-Auth

API:et förväntar sig följande claims i JWT-token:

- **User ID**: `sub`, `id`, `userId`, eller `ClaimTypes.NameIdentifier`
- **Username**: `username` eller `ClaimTypes.Name`
- **Email**: `email` eller `ClaimTypes.Email`
- **Email Verified**: `emailVerified`, `email_confirmed`, eller `emailConfirmed`
- **Subscription Tier** (optional): `subscriptionTier` eller `subscription_tier`

Om better-auth använder andra claim-namn, uppdatera `ClaimsPricipalExtensions.cs`.

## API Endpoints

### Användarhantering

#### `GET /api/users/me`
Hämta inloggad användares profil
- Kräver: Bearer token
- Returnerar: UserDto

#### `PUT /api/users/me`
Uppdatera inloggad användares profil
- Kräver: Bearer token
- Body: UpdateUserRequest
- Returnerar: UserDto

#### `GET /api/users/{userId}`
Hämta användarprofil via ID (publik)
- Ingen autentisering krävs
- Returnerar: UserDto

#### `GET /api/users/username/{username}`
Hämta användarprofil via användarnamn (publik)
- Ingen autentisering krävs
- Returnerar: UserDto

## Authorization

Alla endpoints som kräver autentisering använder `[Authorize]` attributet:

```csharp
[Authorize]
public async Task<ActionResult> ProtectedEndpoint()
{
    var userId = User.GetUserId(); // Hämtar userId från JWT claims
    // ...
}
```

## Testning

### Med Swagger
1. Logga in via better-auth på frontend
2. Kopiera JWT-token från browser (localStorage eller cookie)
3. I Swagger, klicka på "Authorize" knappen
4. Ange: `Bearer {din-token}`
5. Testa endpoints

### Med Postman/Thunder Client
```
GET /api/users/me
Headers:
  Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Felsökning

### 401 Unauthorized

**Problem:** JWT-token valideras inte

**Lösningar:**
1. Kontrollera att SecretKey är samma i API och better-auth
2. Kontrollera att token inte har gått ut
3. Kontrollera att token skickas i Authorization header som "Bearer {token}"
4. Kolla API-loggar för specifika valideringsfel

### User ID not found in token claims

**Problem:** API:et kan inte hitta user ID i JWT claims

**Lösning:**
1. Inspektera JWT-token på https://jwt.io
2. Kontrollera vilket claim-namn better-auth använder för user ID
3. Uppdatera `ClaimsPricipalExtensions.GetUserId()` om nödvändigt

### CORS-fel

**Problem:** Frontend kan inte anropa API:et

**Lösning:**
Uppdatera CORS-policy i `ServiceCollectionExtensions.cs`:

```csharp
policy.WithOrigins("http://localhost:3000", "https://your-production-domain.com")
```

## Migration från gammal auth

Om du har befintliga användare i databasen:

1. **Användare behöver logga in igen** via better-auth
2. Better-auth kommer att hantera lösenord och sessions
3. API:et kommer endast validera JWT-tokens från better-auth
4. Gamla refresh tokens i databasen används inte längre

## Säkerhet

- **Secret Key**: Använd en stark, slumpmässig secret (minst 256 bits)
- **HTTPS**: Använd alltid HTTPS i produktion
- **Token Expiry**: Konfigurera lämplig expiry-tid i better-auth
- **Environment Variables**: Lagra aldrig secrets i source control

```bash
# I produktion, använd environment variables:
export BetterAuth__Jwt__SecretKey="your-production-secret"
```

## Support

Om du stöter på problem:
1. Kontrollera API-loggar
2. Inspektera JWT-token på jwt.io
3. Verifiera better-auth konfiguration
4. Kontrollera att alla claims finns i token
