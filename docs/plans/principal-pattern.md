# Principal Pattern — Plan för Teams, Stores & Polymorft Ägarskap

> **Status:** Designspec — ej implementerad. Sparad för senare när Teams/Stores blir aktuella.
> **Skapad:** 2026-05-07
> **Förberedande nu (nivå 1):** Använd `PrincipalId` som kolumnnamn på Cars/Journeys/Inventory redan när de byggs (FK pekar på Users-tabellen tills vidare). Ingen annan infrastruktur behövs förrän teams faktiskt byggs.

---

## 1. Vision

Paddokk ska stödja:
- **Personliga** resurser (en User äger sin bil, sin journey, sin inventory)
- **Team** (en grupp users som tillsammans äger team-bilar, team-journeys, etc.)
- **Stores** (kommersiella entiteter med produkter, kampanjer, marketplace-listings)

Dessa tre entitetstyper behöver kunna *äga* resurser på samma sätt. Lösningen är en polymorf ägarmodell — `Principal` som abstrakt bas, med `User`, `Team` och `Store` som specialiseringar.

---

## 2. Datamodell

### 2.1 Principal-hierarki (TPT-arv i EF Core)

```
                    ┌─────────────────┐
                    │    Principal    │   abstrakt bas
                    │ Id, CreatedAt   │   (egen tabell)
                    │ Slug (unique)   │
                    └────────┬────────┘
                             │
            ┌────────────────┼────────────────┐
            ▼                ▼                ▼
       ┌─────────┐      ┌─────────┐      ┌──────────┐
       │  User   │      │  Team   │      │  Store   │
       │ Email   │      │ Name    │      │ OrgNr    │
       │ Display │      │ Avatar  │      │ Payout   │
       └─────────┘      └─────────┘      └──────────┘
```

**Viktigt:** `User.Id == Principal.Id` (samma GUID, två tabellrader). När EF Core skapar en User skrivs först en rad i `Principals`, sedan en rad i `Users` med samma Id. Detta är transparent när man använder `db.Users.Add(user)`.

**Slug** placeras på `Principal`-bastabellen så att unikhet enforce:as över alla Principal-typer (`/tobias`, `/volvoklubben`, `/bilshoppen` kan inte krocka).

### 2.2 Membership-tabell

```
   Memberships
   ┌──────────────┬───────────────────┬──────────┬─────────────┐
   │ UserId       │ PrincipalId       │ Role     │ JoinedAt    │
   ├──────────────┼───────────────────┼──────────┼─────────────┤
   │ 0a1b-tobias  │ 9e8f-volvoklubben │ Owner    │ 2026-01-15  │
   │ 5c6d-anna    │ 9e8f-volvoklubben │ Member   │ 2026-02-01  │
   │ 5c6d-anna    │ b4c2-bilshoppen   │ Owner    │ 2026-03-10  │
   └──────────────┴───────────────────┴──────────┴─────────────┘
```

- `UserId` → FK till User
- `PrincipalId` → FK till Principal (måste vara Team eller Store, inte User — enforce:as i applikationskoden)
- `Role` enum: `Owner | Admin | Member | Viewer`
- Unique index: `(UserId, PrincipalId)` — en user kan ha max en roll per principal

### 2.3 Resurser

Alla "ägbara" resurser refererar till `PrincipalId` istället för `UserId`:

```
   Cars: { Id, PrincipalId, Make, Model, ... }
   Journeys: { Id, PrincipalId, Title, ... }
   InventoryItems: { Id, PrincipalId, ... }
   Products: { Id, PrincipalId, ... }       — bara Stores
   Campaigns: { Id, PrincipalId, ... }      — bara Stores
   Posts: { Id, JourneyId, AuthorId }       — AuthorId alltid User
```

**Typrestriktioner per resurs:**

| Resurs | Giltig Principal-typ |
|---|---|
| Car, Journey, InventoryItem | User, Team |
| JourneyPost | Ägs av journey:s Principal; AuthorId är alltid User |
| Product, Campaign | Store |
| MarketplaceListing | User (only — beslut nedan) |

**Skilj på OwnerId och AuthorId:**
- `PrincipalId` = vem som äger resursen (styr behörighet)
- `AuthorId` = vem som skapade en specifik post/kommentar (alltid User; för audit/UI)

En team-journey kan ha posts skrivna av olika medlemmar — `Journey.PrincipalId = Team`, men `JourneyPost.AuthorId = User` som faktiskt skrev.

---

## 3. Behörighet

### 3.1 Roller och permissions

| Permission | Owner | Admin | Member | Viewer |
|---|---|---|---|---|
| Read resources | ✓ | ✓ | ✓ | ✓ |
| Create journey/car/etc | ✓ | ✓ | ✓ | ✗ |
| Edit any resource | ✓ | ✓ | ✗* | ✗ |
| Delete resource | ✓ | ✓ | ✗ | ✗ |
| Invite members | ✓ | ✓ | ✗ | ✗ |
| Change roles | ✓ | ✗ | ✗ | ✗ |
| Delete principal | ✓ | ✗ | ✗ | ✗ |

\* Member får editera resurser de själva skapat (`CreatedByUserId` på resursen avgör).

### 3.2 PrincipalAccessService

Hela behörighetssystemet bygger på en enda metod:

```csharp
public sealed class PrincipalAccessService
{
    public async Task<bool> CanActAsync(
        Guid userId,
        Guid principalId,
        Permission permission)
    {
        if (userId == principalId) return true;  // personlig resurs

        var membership = await _db.Memberships
            .FirstOrDefaultAsync(m =>
                m.UserId == userId && m.PrincipalId == principalId);

        return membership is not null
            && membership.Role.Allows(permission);
    }
}
```

Tillämpas via MediatR pipeline behavior på commands som implementerar `IPrincipalScopedRequest`.

### 3.3 Multiple Owners

Ett team/store kan ha **flera Owners** (likt GitHub Orgs). Co-founder-mönster.

**Invariant:** `COUNT(Memberships WHERE PrincipalId=X AND Role=Owner) >= 1` — alltid.

Tre operationer kan bryta invarianten — alla tre måste validera:
- `LeaveTeamCommand` (Owner lämnar)
- `RemoveMemberCommand` (Owner kickas)
- `ChangeMemberRoleCommand` (Owner demoteras)

Lyft ut till `PrincipalOwnershipInvariant`-helper.

**Konsekvenser av jämlika Owners:**
- Owner kan kicka annan Owner
- Owner kan demotera annan Owner
- Vilken Owner som helst kan radera teamet (bekräftelse i UI, inte i domän)

---

## 4. Skapande av Team/Store

När en User skapar ett Team/Store:

```csharp
public async Task<Guid> Handle(CreateTeamCommand cmd)
{
    var team = new Team
    {
        Id = Guid.NewGuid(),       // nytt PrincipalId — INTE samma som CreatorUserId
        Name = cmd.Name,
        Slug = cmd.Slug,
    };
    _db.Teams.Add(team);           // EF skapar Principals-rad + Teams-rad

    _db.Memberships.Add(new Membership
    {
        UserId = cmd.CreatorUserId,
        PrincipalId = team.Id,
        Role = TeamRole.Owner,     // skaparen blir Owner via membership
        JoinedAt = DateTimeOffset.UtcNow,
    });

    await _db.SaveChangesAsync();
    return team.Id;
}
```

**Viktigt:** Teamet får sitt **eget** PrincipalId, helt skilt från skaparens UserId. Skaparens koppling till teamet sker uteslutande via Membership.

---

## 5. Active Workspace (UI/BFF-lager)

User-sessionen håller en `activeOwnerId`:
- Default = User.Id (personlig workspace)
- Kan växlas till Team/Store där usern är medlem

Listnings-endpoints filtrerar på `activeOwnerId`. Detail-endpoints kollar access oavsett aktiv workspace (djuplänkar fungerar).

UI: workspace-switcher i header (likt Linear, GitHub Orgs, Notion).

---

## 6. Edge Cases — Beslut

### A. Konto-radering (GDPR)
- Anonymisera User-rad, behåll Principal-Id som tombstone
- Email → null, DisplayName → "Borttagen användare", IsDeleted = true
- Memberships, Author-referenser, etc. behåller giltiga FK:er
- **Pre-check:** Om sista Owner i något team → tvinga transfer först

### B. Member lämnar team där deras bil är kopplad till team-journey
- Bara Membership-raden raderas. **Inget annat ändras.**
- Bil-länkarna i team-journeys finns kvar (historisk fakta)
- Anna kan inte längre se team-internt, men behåller full kontroll över sin egen bil
- Om hon sätter sin bil till privat eller raderar den → embedden faller tillbaka till "🔒 Privat fordon" eller "Bil borttagen" (bilens egen privacy/existens vinner alltid)

### C. Slug-unikhet över Principal-typer
- `Slug` ligger på Principal-bastabellen med unique index
- Omöjliggör krockar mellan User-, Team- och Store-slugs

### D. Synlighet
- **Team är alltid Public** (visas i sökresultat och listningar)
- **Anslutning är alltid via invitation** — ingen self-service join
- "Public" betyder bara *visningssynlig*, inte *anslutbar*
- Bil-/journey-privacy: bara `Public` eller `Private` (under wraps). Bil-centrerat innehåll, ej privacy-tungt.

### E. Inaktiv sista Owner ("bus factor")
- **Hanteras via support-kontakt** tills vidare
- Designa så att framtida auto-promote kan läggas till utan datamodellsändring

### F. Författarskap efter att medlem lämnat teamet
- Posts skapade i team-kontext **ägs av teamet** — användaren kan inte editera/radera dem efter att ha lämnat
- UI visar text vid postande: *"You are acting on behalf of the team, and as such you do not have the right to have your post removed unless the team gives you the right"*
- AuthorId behålls för historik; UI visar "(tidigare medlem)" om användaren inte är medlem längre

### G. Team-byte av namn / slug
- `SlugHistory`-tabell sparar gamla slugs
- Gamla URL:er redirectar till nuvarande i 6 månader
- Förhindra omedelbar återanvändning av frigjorda slugs (squatting-skydd)

### H. Aktivitetsflöde
- Logga både `ActorUserId` och `OnBehalfOfPrincipalId`
- UI väljer formulering beroende på vy:
  - User-feed: "Anna lade till X i Volvoklubben"
  - Team-feed: "Volvoklubben fick en ny bil (av Anna)"

### Cross-principal-resurser (regel)
- En **user-journey kan inte använda team-bilar**
- En **team-journey kan använda user-bilar** för medlemmar (med godkännande — se sektion 8)
- Regeln: `Resource.PrincipalId` måste vara samma som journey:s `PrincipalId`, *eller* user-bilen är godkänd för team-koppling

### Marketplace Listings
- Bara Users skapar listings (ej Stores i v1)
- Paddokk hanterar inte transaktioner — bara annonser

### Follows / Likes
- Endast Users följer/gillar (ej Teams/Stores som aktörer)

### Borttagning av Team/Store
- Resursberoende:
  - Cars, Journeys → Owner får välja: transferera till sin egen User-Principal eller annan, eller radera
  - InventoryItems → raderas
  - Posts → raderas (de hör till teamet)
- Pending invites auto-revokas

---

## 7. Invitations

### 7.1 Två sätt att bjuda in

**Direktinbjudan (riktad):** E-post eller användarnamn → en specifik person
**Delbar länk:** Token-URL som flera kan använda (med ev. max-uses + expiry)

### 7.2 Datamodell

```
   Invitation                     InviteLink
   ┌────────────────────────┐    ┌────────────────────┐
   │ Id                     │    │ Id                 │
   │ PrincipalId            │    │ PrincipalId        │
   │ Role                   │    │ Role               │
   │ InvitedByUserId        │    │ Token (unik)       │
   │ InviteeEmail (null)    │    │ CreatedByUserId    │
   │ InviteeUserId (null)   │    │ MaxUses (null)     │
   │ Token (unik)           │    │ UsesCount          │
   │ Status                 │    │ ExpiresAt (null)   │
   │ CreatedAt              │    │ RevokedAt (null)   │
   │ ExpiresAt              │    │ CreatedAt          │
   │ ActedAt (null)         │    └────────────────────┘
   └────────────────────────┘

   Status: Pending | Accepted | Declined | Expired | Revoked
```

### 7.3 State-maskin (Invitation)

```
   Pending
      ├── (anna accepterar) ──► Accepted ──► Membership skapas
      ├── (anna avböjer)    ──► Declined
      └── (tid > expiresAt
           eller admin
           återkallar)      ──► Expired / Revoked
```

Endast `Pending` → `*` är giltig övergång. Slutstatus är permanent.

### 7.4 Sender-roll-begränsning

Inviterande får bara skicka inbjudan till roller **lägre eller lika** sin egen.
- Member kan inte invitera (saknar `InviteMembers`-permission)
- Admin kan invitera Member/Viewer
- Owner kan invitera vad som helst

Förhindrar promotion-kedjor genom inbjudningar.

### 7.5 Beslut

| # | Beslut |
|---|---|
| A | Default expiry: **14 dagar**. Nattjobb flippar Pending → Expired. |
| B | Personen är redan medlem → **block direkt** vid send-time |
| C | Dubbel-inbjudan → **refresh** (gamla → Revoked, ny skapas) |
| D | (utgår — ingen self-service join) |
| E | Team raderas med pending invites → **auto-revoke alla** |
| F | Notifikationer: in-app + e-post för accept/skicka, in-app tystare för decline |

---

## 8. Föreslå-och-godkänn (delat mönster)

Två (potentiellt fler) ställen i appen behöver "aktör föreslår, mottagare godkänner":

1. **Invitations** — Admin föreslår, user godkänner (sektion 7)
2. **Bil-koppling till team-journey** — Team-Admin föreslår att Annas bil läggs i team-journey, Anna godkänner

Övervägd som *delad infrastruktur* (en generisk `Proposal`-tabell). **Beslut: hålls separata tills vidare** — distinkt domänsemantik, distinkta fält. Kan refaktoreras till delat mönster om det dyker upp 3+ instanser.

### 8.1 Bil-koppling till team-journey (Alt C)

```
   Tobias (Team-Admin) skapar journey "Mantorp 2026"
              │
              ▼
   Vill lägga till "Annas Volvo 244" som deltagande bil
              │
              ▼
   Skapas: JourneyCarProposal { JourneyId, CarId, ProposedBy, Status=Pending }
              │
              ▼
   Notis till Anna: "Volvoklubben vill lägga till din Volvo i Mantorp 2026"
              │
              ▼
   Anna [Acceptera] / [Avböj]
              │
        ┌─────┴─────┐
        ▼           ▼
   ✅ Skapas    ❌ Status:
   JourneyParti   Declined
   cipatingCar
```

**Bara bilägaren själv** kan slutligen koppla bilen — Admin kan bara *föreslå*.

---

## 9. Migrationsstrategi (när teams faktiskt byggs)

### Förutsättning: Nivå 1-förberedelse är gjord
- Alla resurstabeller har redan `PrincipalId`-kolumn (FK till Users tills vidare)
- Inga column-renames behöver göras

### Migrationssteg

1. **Skapa Principals-tabell** med kolumner: Id, CreatedAt, Slug
2. **Backfill:** kopiera alla User.Id → Principal-rader (samma GUID, Kind=User)
3. **Konfigurera TPT-arv** i EF: `User : Principal`
4. **Migrera Slug** från User till Principal-bastabell
5. **Ändra FK:n** på alla resurstabeller: `PrincipalId → Principals.Id` (inte längre Users.Id)
6. **Skapa Teams + Stores-tabeller** (TPT-syskon till User)
7. **Skapa Memberships-tabell**
8. **Skapa Invitation + InviteLink-tabeller**
9. **Bygg PrincipalAccessService**
10. **Refaktorera commands/queries** stegvis: byt ut `WHERE UserId = @me` mot `PrincipalAccessService.CanActAsync()`
11. **Bygg UI:** workspace-switcher, team-skapande, invitations, member management
12. **Lansera Stores** (separata kommandon för Product/Campaign-resurser)

### Riskpunkter
- **Steg 5** (FK-migration) — gör i en transaction, validera FK-integritet före commit
- **Steg 10** (refaktorering) — gör per resurstyp, inte allt på en gång. Behåll bakåtkompatibla queries under övergången
- **Steg 4** (Slug-flytt) — kontrollera att inga User-slugs krockar med planerade Team/Store-slugs

---

## 10. Naming Conventions

För att undvika tvetydighet:

| Term | Betydelse |
|---|---|
| **Principal** | Polymorf bas-entitet (User/Team/Store) |
| **Owner (entitet)** | Ej använd — ersätts av "Principal" |
| **Owner (roll)** | Membership.Role-värde — högsta nivån |
| **PrincipalId** | FK på resurser, pekar på Principals.Id |
| **AuthorId** | FK på posts/comments, pekar alltid på User |
| **Membership** | Kopplingstabell User ↔ Principal med roll |

Domänkod: använd `Principal` konsekvent. Användarvänd UI-text: "Owner" är OK för rollen ("Du är Owner av Volvoklubben").

---

## 11. Öppna frågor (att besluta vid implementation)

- [ ] Custom permissions per Membership (utöver roll)? T.ex. "Cashier" i Store. *Nuvarande hållning: nej — generiska roller räcker.*
- [ ] Audit-logg på role changes och membership-borttagningar?
- [ ] Soft-delete vs hard-delete på Cars (för B-edge-case)? *Förslag: soft-delete med 30 dagars grace period.*
- [ ] E-post-template för invitations (transactional mail-leverantör)?
- [ ] Rate limiting på invitation-skick (anti-spam)?
- [ ] Plural begränsning: max antal team per User? Max medlemmar per team? *Förslag: ingen i v1, lägg till om missbruk uppstår.*

---

## 12. Vad ska göras NU (under user-only-fasen)

**Bara en sak:** Använd `PrincipalId` som kolumnnamn på alla nya ägar-FK:er.

```csharp
public class Car
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }  // ← inte UserId
    // FK pekar på Users-tabellen tills vidare
    // ...
}
```

Allt annat — Principal-bastabell, Memberships, PrincipalAccessService, Teams/Stores, Invitations — väntar tills feature-arbetet faktiskt börjar.
