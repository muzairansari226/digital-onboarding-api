# Digital Onboarding API

A .NET 8 Web API implementing the customer onboarding journey, built on
Clean Architecture with Entity Framework Core over SQL Server.

The design has two lanes. They differ **only at the first step** — registration creates an account,
migration resolves one that already exists — and share every step after it. There is therefore one
set of OTP, consent, PIN and biometric endpoints, not two.

| Step | Registration lane | Migration lane |
|------|-------------------|----------------|
| 1 | requirements checklist → account form | mobile lookup |
| 2–6 | mobile OTP → email OTP → privacy policy → PIN → biometric | *identical* |
| 7 | landing screen | *identical* |

---

## Getting started

**Prerequisites:** .NET 8 SDK (or newer) and SQL Server (LocalDB is fine).

```bash
# 1. Create the connection-string environment variable (see below).
set-connection-string.bat

# 2. Restore the local dotnet-ef tool.
dotnet tool restore

# 3. Create the database and apply the schema.
dotnet ef database update --project CustomerPortal.Infrastructure --startup-project CustomerPortal.Api

# 4. Run. Swagger is at /swagger, in Development only.
dotnet run --project CustomerPortal.Api
```

### The connection string

It lives in an **environment variable**, never in a file. `appsettings.json` holds only the *name*
of that variable:

```json
"EnvironmentVariables": { "SqlConnection": "CUSTOMERPORTAL_SQLCONNECTION" }
```

so no credential is ever committed to source control. Run **`set-connection-string.bat`** from the
repository root to create it. With no argument it uses a local SQL Server default:

```
Data Source=localhost;Initial Catalog=DigitalOnboarding;Integrated Security=True;...
```

Pass your own to override:

```bat
set-connection-string.bat "Server=myhost;Database=DigitalOnboarding;User Id=app;Password=..."
```

The script persists the variable for your Windows user, sets it in the current shell, and refuses a
connection string with no `Initial Catalog=` or `Database=` — without a database name,
`dotnet ef database update` has nothing to create and would build the schema in `master`.

**Terminals, Visual Studio and IIS that are already open will not see the variable until they are
restarted.** If the API starts and immediately throws
`The environment variable 'CUSTOMERPORTAL_SQLCONNECTION' does not hold a SQL connection string`,
that restart is what is missing.

### The content tables start empty

There is **no seed data**, by design. Three screens are content-driven, and until their tables hold
rows the matching endpoints return an empty list or a 404:

- `GET /api/v1/onboarding/requirements` → `[]`
- `GET /api/v1/consents/documents/PrivacyPolicy` → 404
- `GET /api/v1/home/{userId}` → an empty `cards` array

The privacy-policy document is the only one the journey cannot proceed without, since a PIN cannot
be set until consent is recorded. To walk the flow locally, insert content by hand — this is a
developer convenience, not seed data, and nothing in the application inserts it for you:

```sql
INSERT INTO ConsentDocument (RecId, DocumentType, Version, Title, Content, EffectiveFrom, IsActive, CreatedAt, UpdatedAt)
VALUES (NEWID(), 1, '1.0', 'Privacy Policy', '<policy text>', SYSUTCDATETIME(), 1, SYSUTCDATETIME(), SYSUTCDATETIME());

INSERT INTO OnboardingRequirement (RecId, Code, Title, Description, DisplayOrder, IsMandatory, IsActive, CreatedAt, UpdatedAt)
VALUES (NEWID(), 'EMIRATES_ID', 'Emirates ID', 'Have your Emirates ID to hand.', 1, 1, 1, SYSUTCDATETIME(), SYSUTCDATETIME());

INSERT INTO HomeContentCard (RecId, Title, Body, ImageUrl, ActionUrl, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
VALUES (NEWID(), 'Welcome', 'Explore what you can do next.', NULL, NULL, 1, 1, SYSUTCDATETIME(), SYSUTCDATETIME());
```

`ConsentDocument.DocumentType` is `1` for `PrivacyPolicy` and `2` for `TermsAndConditions`.

> **Running manual DML with `sqlcmd`?** Pass the `-I` flag. `User`, `UserPin` and `UserDevice`
> carry filtered unique indexes, and SQL Server refuses `INSERT`/`UPDATE`/`DELETE` against such a
> table unless `QUOTED_IDENTIFIER` is `ON` — which `sqlcmd` leaves `OFF` by default. The three
> content tables above have no filtered index, so their inserts work either way. The application
> is unaffected: `Microsoft.Data.SqlClient` sets the option `ON` for every connection.

---

## Endpoints

All routes are versioned and the version is mandatory: `api/v1/...`. Every response — success,
expected failure and unexpected failure alike — uses the same `ApiResponse<T>` envelope.

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `api/v1/onboarding/requirements` | the "what you'll need" checklist |
| GET | `api/v1/onboarding/registrations/{registrationId}` | progress, so an interrupted journey resumes on the right screen |
| POST | `api/v1/registration/availability` | is this email or mobile already taken |
| POST | `api/v1/registration/start` | create a pending account → `registrationId` |
| POST | `api/v1/migration/start` | resolve an existing account → `registrationId` |
| POST | `api/v1/otp/send` | issue a code; **also serves resend**, cooldown-guarded |
| POST | `api/v1/otp/verify` | check a code, mark the channel verified |
| GET | `api/v1/consents/documents/{documentType}` | the policy text and its version |
| POST | `api/v1/consents` | record acceptance |
| POST | `api/v1/pin/set` | set the PIN and activate the account |
| POST | `api/v1/pin/verify` | check a PIN |
| POST | `api/v1/biometric/enroll` | bind a device, returning its secret once |
| GET | `api/v1/users/{userId}` | profile |
| PUT | `api/v1/users/{userId}` | update the name on a profile |
| GET | `api/v1/users/{userId}/devices` | enrolled devices |
| GET | `api/v1/home/{userId}` | landing-screen name and content cards |

`registrationId` **is** the user's `RecId`. There is no session entity: each service re-derives its
own precondition from the account's verification flags and the existence of its consent, PIN and
device rows, so there is no journey state that can fall out of sync with reality.

There is no separate resend endpoint — a resend is `otp/send` called again, which is exactly what
the cooldown, the per-window cap and the retirement of the outstanding challenge exist to govern.

### The error screens

The red frames in the design are expected failures, and carry machine-readable detail:

| Screen | Response |
|--------|----------|
| Account already exists | `409` |
| Incorrect OTP | `400`, with `data.attemptsRemaining` counting down |
| Out of OTP attempts | `429`, the challenge is burned |
| Resend too soon | `429`, with the wait in the message |
| Incorrect PIN | `400`, with `data.attemptsRemaining` |
| PIN locked out | `423`, with `data.lockedUntil` |

---

## Configuration

`appsettings.json` holds the **name** of the environment variable carrying the connection string,
never the value, so no credential is ever committed. Everything else tunable lives
in bound sections — `Otp`, `Pin`, `Hashing`, `Onboarding`, `Database`, `RateLimiting` — and reaches the
services as `IOptions<T>`. No limit, timeout or work factor is written as a literal in code.

The OTP and PIN lengths default to **4 digits**, matching the designs. Both are settings, so
raising them needs no code change.

### `Otp:ReturnCodeInResponse`

With no SMS or email provider integrated yet, this flag echoes the generated code back in the
`otp/send` response so the journey can be walked from Swagger. It is `true` in
`appsettings.Development.json` and **`false` everywhere else** — it must stay that way anywhere a
real customer can reach the API.

The delivery integration point is `Infrastructure/Providers/OtpSender/OtpSenderProvider.cs`. Its
two private methods, `SendSmsAsync` and `SendEmailAsync`, are the single place a real gateway needs
to be plugged into.

---

## Security notes

**There is deliberately no auth layer** — no token middleware, no `[Authorize]`, no permission
claims. Every endpoint in this journey is reachable before the customer is known: you cannot demand
a token to register. The absence is a decision, not an oversight.

What protects the journey instead lives inside the services:

- **PINs and OTP codes are never stored.** Both are Argon2id hashes over a per-secret salt, compared
  in fixed time.
- **OTP codes** are drawn from `RandomNumberGenerator`, expire in minutes, are single use, are
  capped on attempts, and issuing a new one retires any still outstanding.
- **PINs** reject repeated digits and straight sequences at set time, and lock the account for a
  cooling-off period after repeated failures. A success resets the counter, so isolated mistakes
  cannot accumulate into a lockout over weeks.
- **Biometrics never reach the server.** The handset keeps the fingerprint or face data in its
  secure enclave; what is exchanged is a device-bound secret, returned once and stored only as a
  hash, revocable per device.
- **Rate limiting** guards the OTP, PIN-verify and registration endpoints per caller IP. The
  per-account limits — cooldown, resend cap, lockout — live in the services, because those need the
  account, which a limiter cannot see without reading the body.
- **Nothing sensitive is logged.** Codes, PINs and full contact details never reach a log; the OTP
  provider logs a masked destination only.

Two items worth settling with the product owner rather than in code:

1. `registration/availability` makes account enumeration possible. The design calls for the
   "account already exists" screen, so the endpoint is rate limited rather than removed — but if
   that matters for this customer base, the usual mitigation is a neutral response with the message
   delivered over SMS or email instead.
2. The landing screen shows per-user data. That is the point at which an auth layer normally
   becomes necessary, and it is far cheaper to decide before the schema is fixed.

---

## Project structure

```
CustomerPortal.Domain/          entities + repository interfaces. No project or package references.
CustomerPortal.Application/     use cases: services, models, contracts, settings, constants.
CustomerPortal.Infrastructure/  DbContext, migrations, repositories, providers.
CustomerPortal.Api/             controllers, exception middleware, validation filter, composition root.
db/procedures/                  hand-written stored procedures (currently none — see its README).
```

Dependencies run inward: `Api → Application, Infrastructure`; `Infrastructure → Application, Domain`;
`Application → Domain`; `Domain → nothing`.

Repository interfaces live in `Domain` and may mention only entities, scalars and tuples — which is
why **projecting an entity into a response model is the service's job**, never the repository's.
Controllers hold no business logic and no `try/catch`: each action calls its service and hands the
envelope to `Result()`. Unexpected failures bubble to `ExceptionHandlingMiddleware`, which logs the
detail with a correlation id and returns only that id to the caller.

### Adding a feature

Entity → `DbSet` → migration → repository interface in `Domain/IRepositories` → repository in
`Infrastructure/Repositories` → `Application/Models/<Feature>Model.cs` →
`Application/Services/<Feature>/` → two DI lines → controller.
