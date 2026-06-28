# Employee Api (.NET 8 Web API)

Three endpoints — Insert, Get, Delete — built with Clean Architecture, CQRS,
and EF Core / SQL Server, designed to plug straight into the React employee
grid app.

## Architecture

```
EmployeeApi.Domain          <- entities only, zero dependencies
        ^
EmployeeApi.Application     <- CQRS commands/queries, handlers, validation, repository interface
        ^
EmployeeApi.Infrastructure   <- EF Core DbContext, repository implementation
        ^
EmployeeApi.Api              <- controllers, Program.cs, middleware
```

Dependencies only point inward: Domain knows nothing about anyone; Application
knows Domain; Infrastructure and Api know Application. The Api project is the
only place EF Core's connection string and Swagger live; nothing in
Application or Domain references EF Core at all.

## CQRS

Reads and writes are modeled as separate objects instead of one bloated
"EmployeeService":

| Type | Class | Purpose |
|---|---|---|
| Command | `CreateEmployeeCommand` → `CreateEmployeeCommandHandler` | Insert |
| Query | `GetAllEmployeesQuery` → `GetAllEmployeesQueryHandler` | Get |
| Command | `DeleteEmployeeCommand` → `DeleteEmployeeCommandHandler` | Delete |

Controllers depend on a single `MediatR.ISender` abstraction and never see a
concrete handler — MediatR resolves the right `IRequestHandler<,>` from DI at
runtime. Validation is a pipeline behavior (`ValidationBehavior<,>`) that runs
every registered FluentValidation validator before the handler executes, so
handlers themselves never call a validator directly. Mapping between
`Employee` and `EmployeeDto` goes through an AutoMapper `Profile`
(`EmployeeMappingProfile`).

### A licensing note

MediatR (v13+) and AutoMapper (v15+) both moved to a dual-license model in
2025: free for individuals/companies under $5M in revenue (the "Community"
tier), paid above that. Either way you register for a free key at
[mediatr.io](https://mediatr.io) / [automapper.io](https://automapper.io) and
drop it into `appsettings.json`:

```json
"MediatR": { "LicenseKey": "your-key-here" },
"AutoMapper": { "LicenseKey": "your-key-here" }
```

Leaving these blank doesn't break anything — both libraries still run fine,
they just log a one-line reminder on startup. If that log noise bothers you
before you've registered a key:

```csharp
builder.Logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);
```

## SOLID, mapped to the actual code

- **Single Responsibility** — each handler does exactly one thing
  (`CreateEmployeeCommandHandler` only creates, never validates HTTP concerns
  or maps responses for other operations — validation lives in
  `ValidationBehavior`, mapping lives in `EmployeeMappingProfile`).
- **Open/Closed** — adding a fourth endpoint means adding one command/query +
  handler + validator. MediatR finds the handler by assembly scan, FluentValidation
  finds the validator the same way — no existing class changes, no new DI line.
- **Liskov Substitution** — every handler implements `IRequestHandler<TRequest,
  TResponse>` consistently, so MediatR can invoke any of them the same way.
- **Interface Segregation** — handlers only ever implement the one
  `IRequestHandler<,>` they need; nothing is forced to implement methods for
  concerns it doesn't have.
- **Dependency Inversion** — `IEmployeeRepository` is defined in Application
  and implemented in Infrastructure; controllers depend on `ISender` and
  `IMapper`, never on EF Core or a concrete handler.

## Endpoints

| Method | Route | Body | Response |
|---|---|---|---|
| GET | `/api/employees` | – | `200` + `[{ id, name, salary, dept }, ...]` |
| POST | `/api/employees` | `{ name, salary, dept }` | `201` + created record |
| DELETE | `/api/employees/{id}` | – | `204`, or `404` if not found |

Validation failures return `400` with an `errors` array (handled centrally by
`ExceptionHandlingMiddleware`, so handlers just throw and don't worry about
status codes).

## Running it locally

1. **Restore packages** (needs internet access to nuget.org):
   ```
   dotnet restore
   ```

2. **Set your connection string.** `appsettings.json` defaults to LocalDB:
   ```
   Server=(localdb)\mssqllocaldb;Database=EmployeeAppDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
   ```
   Point it at a different SQL Server instance if you're not using LocalDB.

3. **Install the EF Core CLI tool** (one-time, if you don't have it):
   ```
   dotnet tool install --global dotnet-ef
   ```

4. **Generate the initial migration:**
   ```
   dotnet ef migrations add InitialCreate -p src/EmployeeApi.Infrastructure -s src/EmployeeApi.Api
   ```

5. **Run the API:**
   ```
   dotnet run --project src/EmployeeApi.Api
   ```
   In Development, `Program.cs` applies any pending migrations automatically
   on startup, so step 4 only needs to be repeated when the model changes —
   you don't need a separate `dotnet ef database update`.

6. **Swagger UI** opens automatically at `https://localhost:5001/swagger`.

## Connecting the React app

CORS is already configured for `http://localhost:5173` (Vite's default port),
and `launchSettings.json` runs this API on `https://localhost:5001` /
`http://localhost:5000` — which matches the `API_BASE_URL` already set in the
React app's `src/App.jsx`. Start both projects and the grid should load real
data with no code changes on either side.

## Project structure

```
EmployeeApi.sln
src/
  EmployeeApi.Domain/
    Entities/Employee.cs
  EmployeeApi.Application/
    Abstractions/Messaging/        (ValidationBehavior)
    Abstractions/Repositories/     (IEmployeeRepository)
    Common/Exceptions/             (NotFoundException)
    Employees/
      EmployeeDto.cs, EmployeeMappingProfile.cs
      Commands/CreateEmployee/, Commands/DeleteEmployee/
      Queries/GetAllEmployees/
    DependencyInjection.cs
  EmployeeApi.Infrastructure/
    Persistence/ (AppDbContext, Configurations, Repositories)
    DependencyInjection.cs
  EmployeeApi.Api/
    Controllers/EmployeesController.cs
    Contracts/CreateEmployeeRequest.cs
    Middleware/ExceptionHandlingMiddleware.cs
    Program.cs, appsettings.json, EmployeeApi.Api.http
```
