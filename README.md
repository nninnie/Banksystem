# 🏦 MyBank - Bankadministrationssystem

<p align="center">
  <img src="image/readme-image.png" alt="MyBank startsida" width="1100"/>
</p>

Ett administrationssystem för bankens anställda (**Admins** och **Cashiers**), byggt med **ASP.NET Core Razor Pages**, **Entity Framework Core** och **ASP.NET Core Identity**.
Systemet hanterar kunder, konton, saldon och transaktioner mot en befintlig bankdatabas (Database First).
Utvecklat som en del av utbildningen Systemutvecklare .NET på YH Akademin.
Projektet är byggt som ett riktigt bankadministrationssystem med fokus på arkitektur, lagerindelning, validering, säkerhet och användarroller.

> Kunderna är inte systemanvändare. Det är bankens anställda som loggar in och arbetar i systemet.

---

## 🌍 Live-demo

**Azure:** *https://bankapp-ninnie-afebhbh6b8cjezg0.swedencentral-01.azurewebsites.net*

---

## 🔐 Inloggning

| Roll | E-post | Lösenord |
|---|---|---|
| Admin | robert.johansson@admin.se | Abc123# |
| Cashier | robert.johansson@cashier.se | Abc123# |

Rollerna och användarna skapas automatiskt (seed) vid uppstart om de inte redan finns.

---

## ✨ Funktionalitet

### 🌐 Startsida (publik)
- KPI-kort med totalt antal kunder, totalt antal konton och totalt saldo
- Tabell med statistik per land: antal kunder, antal konton och totalt saldo
- Klickbar karta och klickbara länder som leder till landssidan
- Kräver ingen inloggning

### 🏆 Topp 10 per land (publik)
- De 10 kunder med högst totalt saldo i valt land, med placering, kundnummer, namn och stad
- KPI-kort med antal toppkunder, genomsnittligt saldo och totalt saldo
- **Response Caching** i 60 sekunder, en version per land (`VaryByQueryKeys = "country"`)
- `[AllowAnonymous]`, eftersom Response Caching bara fungerar för utloggade användare

### 🔎 Snabbsökning på kundnummer
- Sökfält i navbaren, synligt för Admin och Cashier
- Kundbilden öppnas direkt om kunden finns, annars visas ett felmeddelande
- Server- och client-side-validering

### 👥 Kundsökning
- Sök på kundnummer eller namn, och på stad
- Visar kundnummer, personnummer, namn, adress och stad
- Sortering stigande/fallande på alla kolumner
- Paginering med 50 kunder per sida (föregående/nästa)
- Klick på en kund öppnar kundbilden

### 👤 Kundbild
- All information om kunden: namn, kundnummer, personnummer, kön, födelsedag, adress, stad, land, telefon och e-post
- Kundens samtliga konton med saldo
- Totalt saldo för kundens konton
- Snabbknappar för insättning, uttag och överföring per konto (Cashier)

### 💳 Kontobild
- Kontonummer, saldo, kontoägare, frekvens och skapandedatum
- Transaktioner sorterade med nyaste först
- **AJAX "Show more"**: hämtar 20 transaktioner i taget med `fetch` utan sidladdning, och knappen döljs när det inte finns fler

### 💰 Insättning, uttag och överföring (Cashier)
- Saldot ändras **aldrig direkt**, bara genom en transaktion
- `decimal` för alla belopp, för att undvika avrundningsfel
- Tydligt felmeddelande om saldot är för lågt vid uttag och överföring
- Överföring skapar två transaktioner (ut från avsändaren, in till mottagaren) i samma `SaveChanges`, så att båda sparas eller ingen
- De 5 senaste transaktionerna på kontot visas under formuläret (partial `_RecentTransactions`)

### 📊 Dashboard (Admin)
- Personlig hälsning efter tid på dygnet
- Statistik: kunder, konton och totalt saldo
- De 5 senaste transaktionerna i banken

### 👤 User Admin (Admin)
- Sida för administration av systemanvändare, skyddad med `[Authorize(Roles = "Admin")]`

### 🛡️ Inloggning, roller och behörighet
- Egen inloggningssida, ingen öppen registrering
- Utloggning sker med ett POST-formulär som skyddas av en antiforgery-token
- **Admin:** dashboard, User Admin, kundsökning, kundbild och kontobild
- **Cashier:** kundsökning, kundbild, kontobild, insättning, uttag och överföring
- `[Authorize(Roles = "...")]` skyddar varje sida, även om någon skriver adressen direkt
- Menyn visar bara det som användarens roll har tillgång till

---

## 🧱 Arkitektur

Lösningen är uppdelad i **tre projekt** (3-lagersarkitektur). Sidorna pratar aldrig direkt med databasen, de går alltid via en service.

```
Razor Page (PageModel)  →  Service  →  DbContext  →  SQL Server
     ↑ ViewModel                ↑ DTO / entitet
     (AutoMapper)
```

**Exempel – överföring:** `Transfer.cshtml.cs` validerar `TransferViewModel` →
`IAccountService.TransferAsync()` kontrollerar saldot och skapar två transaktioner →
`BankAppContext.SaveChangesAsync()` sparar båda på en gång, eller ingen.

### **BankApp – Webbapplikation**
- Tar emot förfrågan, validerar indata i ViewModels och visar resultatet. Innehåller ingen affärslogik.
- Razor Pages (`.cshtml` + PageModel)
- ViewModels med validering
- AutoMapper-profil (`MappingProfile`)
- Helpers: landsflaggor (`CountryFlags`) och visningsnamn för inloggad användare (`UserDisplay`)
- `Program.cs`: dependency injection, Identity, Response Caching och migrering vid uppstart

### **BankApp.Services – Affärslogik**
- Bankens regler: kontrollerar saldo, skapar transaktioner och returnerar en ErrorCode när något inte är tillåtet.
- `ICustomerService`, `IAccountService`, `ITransactionService`, `IDashboardService`
- All logik för insättning, uttag och överföring
- `ErrorCode` (enum) som PageModel översätter till felmeddelanden

### **BankApp.DataAccessLayer – Data**
- Pratar med databasen via EF Core. Entiteter och DbContext lämnar aldrig det här lagret till vyerna.
- Entiteter och `BankAppContext` (Database First)
- `ApplicationDbContext` och migrering för Identity
- DTO:er
- `SeedData` för roller och användare

---

## ✅ Validering

- Data annotations på ViewModels (`[Required]`, `[Range]`, `[StringLength]` m.fl.) med egna felmeddelanden
- **Server-side:** `ModelState.IsValid` kontrolleras innan något sparas eller söks
- **Client-side:** jQuery Validation (installerat med LibMan i `wwwroot/lib`) via `_ValidationScriptsPartial` på alla formulär: inloggning, snabbsökning, kundsökning, insättning, uttag och överföring
- Regler som kräver databasen (t.ex. för lite pengar eller ett mottagarkonto som inte finns) returneras som `ErrorCode` från servicen och visas med `ModelState.AddModelError`
- Ett kund- eller kontonummer som inte finns i adressen ger 404

---

## 🎨 Design

- Bygger på **Corona Admin (Free Bootstrap Template)**, omgjord med egen stilmall (`wwwroot/my-css/style.css`)
- Mörka skogsgröna ytor, guld som primärfärg och grönt som accent
- Färgerna ligger som CSS-variabler i `:root`, så de kan ändras på ett ställe
- Typsnittet Inter med tabulära siffror, så att belopp linjerar i kolumner
- Egen logga och tagline: *TRUST • SECURITY • GROWTH*

---

## 🧪 Tekniker

- .NET 10 / ASP.NET Core Razor Pages
- Entity Framework Core 10 (Database First) + LINQ
- ASP.NET Core Identity (roller: Admin, Cashier)
- AutoMapper
- SQL Server
- JavaScript (`fetch` / AJAX)
- jQuery Validation
- Bootstrap (Corona Admin Free)
- Git & GitHub (en branch per funktion)

---

## 📁 Projektstruktur

```
BankApp/                     Webbapplikationen
  Pages/                     Razor Pages (Admin/, Shared/)
  ViewModels/                ViewModels med validering
  Mapping/                   AutoMapper-profil
  Helpers/                   CountryFlags, UserDisplay
  wwwroot/my-css, my-js      Egen CSS och JavaScript
BankApp.Services/            Services, interfaces, ErrorCode
BankApp.DataAccessLayer/     Entiteter, DbContexts, DTO:er, migrering, seed
image/                       Bilder till README
BankApp.slnx
README.md
```

---

## 🛠️ Installation (F5 mot egen databas)

**Krav:** Visual Studio 2026, .NET 10 SDK, SQL Server

1. Återställ `BankAppDatav2.bak` i SQL Server Management Studio
2. Kör (byt till ditt databasnamn):
   ```sql
   ALTER AUTHORIZATION ON DATABASE::[databasnamn] TO [sa]
   ```
3. Öppna `BankApp/appsettings.json` och ändra `Server=` och `Database=` i **båda** connection strings (`DefaultConnection` och `IdentityConnection`)
4. Öppna `BankApp.slnx`, kontrollera att **BankApp** är startprojekt och tryck **F5**

Vid första start skapas Identity-tabellerna automatiskt (`MigrateAsync` i `Program.cs`), och därefter seedas roller och användare. Banktabellerna finns redan i `.bak`-filen.

---

## 🧩 Lärdomar & Arbetssätt

Jag arbetade enligt principer som:

- **Separation of concerns:** sida, logik och data i olika projekt
- **DRY:** partials och services som återanvänds, och flagglogiken samlad på ett ställe
- **En branch per funktion:** merge till master när funktionen är testad
- **Validering från början:** varje ny sida byggs med server- och client-side-validering direkt

Lärdomar:

- Jag borde ha skapat klassbiblioteken och planerat lagren tidigare
- I början använde jag ViewModels där DTO:er passade bättre
- Git-mappen låg först inne i webbprojektet, så klassbiblioteken kom inte med till GitHub. Jag flyttade upp repot en nivå
- Statistiken får bara räkna kontoägare (`OWNER`), annars räknas delade konton dubbelt
- Utan `MigrateAsync` vid uppstart hade appen kraschat mot en nyss återställd databas
- Validera först, ändra data sedan: ordningen i OnPost spelar roll
- Nästa gång gör jag moodboard och sidplanering innan jag designar

---

## ✍️ Utvecklat av
**Ninnie Dahlström – Systemutvecklare .NET, YH Akademin**
