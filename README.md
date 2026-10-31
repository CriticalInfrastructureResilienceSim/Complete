# Critical Infrastructure Resilience Simulator (CIRS)

University Data Structures and Algorithms project. ASP.NET Core MVC on .NET 8, EF Core with MySQL, Serilog.
All data is synthetic.

## What is implemented now (Week 1, login and sign up only)

- Sign up, log in, log out, cookie authentication ("Remember me" optional), small session state
- Passwords stored only as BCrypt hashes, account lock after repeated failed logins
- Validation in its own classes (`Validators/`), wording in `ValidationMessages`, help text in `FieldHelp`
- Shared `_Layout`, `_AuthLayout`, `_FormField` and `_FormErrors` so every form looks and behaves the same
- Redesigned landing page (hero with cascade illustration, the three questions, how it works, data structures and algorithms, project status) and matching header
- Split-screen login and sign up pages (responsive, no external CSS, fonts or icon downloads): icons inside inputs,
  show/hide password, live password strength bar and rule checklist, loading state on submit, focus moves to the first problem
- Serilog to console and a daily rolling file `logs/app-<date>.log` (Information, Warning and Error together), always called through `IAppLogger`
- `ExceptionHandlingMiddleware`: friendly error page with a reference id, full exception in the log
- Anti-forgery token on every POST
- xUnit tests for the validators

Empty on purpose (filled later): Facility CRUD (`Views/Facilities`, `Models/Facility`, controller, service, validator),
`Cirs.Core` (`DataStructures`, `Graphs`, `Simulation`), `Data/Migrations`.

## Folder structure

```
CriticalInfrastructure.sln
docs/
  database/schema.sql      MySQL script for MySQL Workbench
  prototype/index.html     HTML demo, kept as a reference only
src/
  Cirs.Web/                MVC project
    Configuration/         AuthOptions (bound from appsettings.json)
    Controllers/           HomeController, AccountController
    Data/                  AppDbContext, Migrations/
    Extensions/            ModelStateExtensions
    Middleware/            ExceptionHandlingMiddleware
    Models/                User
    Services/              AuthService, AppLogger (Serilog wrapper) and interfaces
    Validators/            UserValidator, FormValidationResult, ValidationMessages, FieldHelp
    ViewModels/            Login, SignUp, FormField, Profile, Error
    Views/                 Account/, Home/, Shared/, Facilities/ (empty)
    wwwroot/               css, js, lib
    Program.cs, appsettings.json
  Cirs.Core/               class library, no web code (empty folders for now)
tests/
  Cirs.Tests/              validator tests (data structure and engine tests later)
```

## How to run

1. Install the .NET 8 SDK and MySQL Server with MySQL Workbench.
2. Open `docs/database/schema.sql` in MySQL Workbench and run it. It creates the `cirs_db` database and the `users` table.
3. Put your real MySQL password in the connection string. Do not commit it; use user-secrets:
   ```
   cd src/Cirs.Web
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=cirs_db;User=root;Password=YOUR_PASSWORD;"
   ```
4. Run:
   ```
   dotnet run --project src/Cirs.Web
   ```
   Then open https://localhost:7080 (or http://localhost:5080).
5. Tests: `dotnet test`

The log file appears in `src/Cirs.Web/logs/` when you run the project from the solution folder as above
(the folder is created relative to where you start the app).

### If you prefer EF migrations instead of schema.sql

Use one route or the other, not both. From `src/Cirs.Web`:
```
dotnet tool install --global dotnet-ef --version 8.0.2
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
```

## Demo script (under five minutes)

1. Sign up with bad data (empty name, weak password, mismatched confirmation): errors appear under each field, help text stays.
2. Sign up with an email that already exists: "An account with this email already exists."
3. Log in with a wrong password: error shown, a Warning appears in the log. After 5 wrong tries the account locks for 5 minutes.
4. Log in correctly, open Profile (shows cookie data and session data), log out.
5. In Development, open `/Home/Crash`: friendly error page with a reference id; find the same id in the log as an Error.
6. Open `logs/app-<date>.log` and show Information, Warning and Error together.
7. Change `Serilog:MinimumLevel:Default` in `appsettings.json`, restart, no recompiling.

## Notes

- Browser forms use `novalidate` on purpose: the server-side validators are the authority.
- Provider note: this uses `Pomelo.EntityFrameworkCore.MySql 8.0.2`, which matches EF Core 8 on .NET 8.
- Never log passwords, hashes or cookies.
