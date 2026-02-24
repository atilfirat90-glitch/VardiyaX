# AGENTS.md

## Cursor Cloud specific instructions

### Project Overview

ShiftCraft (VardiyaX) is a workforce shift management application with a **Clean Architecture** .NET monorepo. The primary developable services on a headless Linux VM are the **ASP.NET Core 8 Web API** and the **xUnit test suite**. The mobile MAUI app (`ShiftCraft.Mobile`) targets `net9.0-android` and cannot be built/run on a headless Linux VM.

### Prerequisites

- **.NET 8 SDK** — installed via `dotnet-install.sh` to `$HOME/.dotnet`. Ensure `DOTNET_ROOT=$HOME/.dotnet` and `PATH` includes `$DOTNET_ROOT`.
- **Docker** — needed to run SQL Server 2022 container for the API database.
- **dotnet-ef** global tool (version 8.0.x) — used to apply EF Core migrations.

### Database Setup (SQL Server via Docker)

```bash
sudo dockerd &  # if not already running
sudo docker run -d --name sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=ShiftCraft2024!" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

After SQL Server is up (~10s), apply migrations:

```bash
cd /workspace/src/ShiftCraft.Api
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
  --project ../ShiftCraft.Infrastructure/ShiftCraft.Infrastructure.csproj \
  --startup-project .
```

**Important:** The EF Core migration only creates the initial schema (tables from `001_InitialCreate`). You must also apply the SQL migration scripts `003`–`005` manually via `sqlcmd` inside the Docker container:

```bash
sudo docker cp src/ShiftCraft.Infrastructure/Migrations/003_AddUsersTable.sql sqlserver:/tmp/003.sql
sudo docker cp src/ShiftCraft.Infrastructure/Migrations/004_AddAuditLogTables.sql sqlserver:/tmp/004.sql
sudo docker cp src/ShiftCraft.Infrastructure/Migrations/005_AddPushNotificationTables.sql sqlserver:/tmp/005.sql

sudo docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'ShiftCraft2024!' -C -d ShiftCraftDb -i /tmp/003.sql
sudo docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'ShiftCraft2024!' -C -d ShiftCraftDb -i /tmp/004.sql
sudo docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'ShiftCraft2024!' -C -d ShiftCraftDb -i /tmp/005.sql
```

**Gotcha:** The admin user seed in `003` requires `Businesses` table to have a row with `Id=1`. Insert a default business first:

```sql
SET IDENTITY_INSERT [dbo].[Businesses] ON;
INSERT INTO [dbo].[Businesses] ([Id],[Name],[Timezone]) VALUES (1,'Default Business','Europe/Istanbul');
SET IDENTITY_INSERT [dbo].[Businesses] OFF;
```

Then re-run the `003` INSERT if needed, or it will already have the Users table created (just no admin row).

### Connection String

The development connection string is configured in `src/ShiftCraft.Api/appsettings.Development.json`:

```
Server=localhost,1433;Database=ShiftCraftDb;User Id=sa;Password=ShiftCraft2024!;TrustServerCertificate=True;MultipleActiveResultSets=true
```

### Running the API

```bash
cd /workspace/src/ShiftCraft.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run --launch-profile http
```

The API listens on `http://localhost:5184`. Swagger UI is available at `/swagger` in Development mode.

### Running Tests

```bash
dotnet test /workspace/tests/ShiftCraft.Tests/ShiftCraft.Tests.csproj
```

Tests use EF Core InMemory provider — they do **not** require SQL Server or Docker.

### Building

```bash
dotnet build /workspace/src/ShiftCraft.sln
```

### Authentication

- **Dev bypass:** `{"username":"admin","password":"admin"}` at `POST /api/auth/login`
- **Config-based:** Username/password from `appsettings.json` `Auth` section (`admin` / `ShiftCraft2024!`)
- Both return a JWT token to use as `Authorization: Bearer <token>`.

### In-App Notification System (v1.3)

The notification system stores notifications in the `Notifications` table. Notifications are created by `PushNotificationService` when:
- A schedule is published (type: `SchedulePublished`)
- A rule violation is detected (type: `ViolationDetected`)
- A shift reminder is due (type: `ShiftReminder`)

API endpoints: `GET /api/notification`, `GET /api/notification/unread`, `GET /api/notification/unread/count`, `POST /api/notification/{id}/read`, `POST /api/notification/read-all`.

**Known issue:** The publish notification flow uses `_userRepository.GetAllAsync()` to find active users, but the resulting `UserId` in notifications may not match the logged-in admin's ID due to EF Core change tracking across the request pipeline. The FK constraint on the `Notifications` table was set to `NO ACTION` to prevent cascading failures.

### Scope Limitations

- `ShiftCraft.Mobile` (MAUI) cannot be built on headless Linux; skip it.
- Appium E2E tests in `tests/package.json` require an Android emulator; not runnable in this environment.
