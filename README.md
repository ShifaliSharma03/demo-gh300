# demo-gh300 — Employee API

A production-ready **ASP.NET Core 9 Web API** for managing employees, featuring CRUD operations, Entity Framework Core with SQLite, Swagger UI, validation, structured logging, unit tests, Docker support, and a GitHub Actions CI pipeline.

---

## Features

| Feature | Details |
|---|---|
| Framework | .NET 9 / ASP.NET Core 9 |
| Database | SQLite via Entity Framework Core 9 |
| API Docs | Swagger UI (served at `/`) |
| Validation | Data Annotations on DTOs |
| Logging | `Microsoft.Extensions.Logging` (Console + Debug) |
| Tests | xUnit with in-memory EF Core (12 tests) |
| Docker | Multi-stage `Dockerfile` |
| CI | GitHub Actions (build → test → Docker build) |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- (Optional) [Docker](https://www.docker.com/)

### Run Locally

```bash
cd EmployeeApi
dotnet run
```

Open **http://localhost:5000** to view the Swagger UI.

### Run Tests

```bash
dotnet test EmployeeApi.slnx
```

### Run with Docker

```bash
docker build -t employee-api .
docker run -p 8080:8080 employee-api
```

Open **http://localhost:8080** to view the Swagger UI.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/employees` | Get all employees |
| GET | `/api/employees/{id}` | Get employee by ID |
| POST | `/api/employees` | Create a new employee |
| PUT | `/api/employees/{id}` | Update an employee |
| DELETE | `/api/employees/{id}` | Delete an employee |

### Example — Create Employee

```json
POST /api/employees
{
  "firstName": "Alice",
  "lastName": "Smith",
  "email": "alice@example.com",
  "department": "Engineering",
  "jobTitle": "Software Engineer",
  "salary": 90000,
  "hireDate": "2024-01-15T00:00:00Z",
  "isActive": true
}
```

---

## Project Structure

```
├── EmployeeApi/
│   ├── Controllers/        # EmployeesController (CRUD)
│   ├── Data/               # AppDbContext (EF Core)
│   ├── DTOs/               # CreateEmployeeDto, UpdateEmployeeDto
│   ├── Models/             # Employee entity
│   ├── Program.cs          # App bootstrap & DI configuration
│   └── appsettings.json    # Connection string & logging config
├── EmployeeApi.Tests/
│   └── EmployeesControllerTests.cs   # 12 unit tests
├── .github/workflows/
│   └── ci.yml              # GitHub Actions CI pipeline
├── Dockerfile
└── EmployeeApi.sln
```

---

## Configuration

The SQLite database path is configured in `EmployeeApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=employees.db"
  }
}
```

Override with an environment variable at runtime:

```bash
ConnectionStrings__DefaultConnection="Data Source=/data/employees.db" dotnet run
```
