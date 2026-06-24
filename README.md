# HR Management System

A production-quality HR Management System built with ASP.NET Core Web API following Clean Architecture principles.

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 8) |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |
| Authorization | Role-Based Access Control (RBAC) |
| CQRS | MediatR |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| API Versioning | Asp.Versioning.Mvc 8 |
| Documentation | Swagger / OpenAPI 3.0 |
| Containerization | Docker + Docker Compose |
| Unit Testing | xUnit + Moq |
| Integration Testing | WebApplicationFactory + EF Core InMemory |

---

## Architecture Overview

The solution is structured into four layers following Clean Architecture. Inner layers never depend on outer layers.

```
HRManagementSystem.sln
├── HRM.Domain          ← Entities, base types, constants
├── HRM.Application     ← Use cases, DTOs, interfaces, validators, MediatR handlers
├── HRM.Infrastructure  ← EF Core, repositories, migrations, interceptors
└── HRM.API             ← Controllers, middleware, DI wiring
```

### Dependency Flow

```
HRM.API  ──►  HRM.Application  ──►  HRM.Domain
   │                                     ▲
   └──────►  HRM.Infrastructure  ────────┘
```

---

## Domain Entities

| Entity | Description |
|---|---|
| `Employee` | Core employee record with salary and job info |
| `Department` | Organizational unit employees belong to |
| `User` | Authentication identity with hashed password and role |
| `AttendanceRecord` | Daily check-in / check-out per employee |
| `PayrollRecord` | Calculated pay per employee per period |
| `AuditLog` | Immutable audit trail of every data change |

---

## API Endpoints

All endpoints are versioned. Current versions: `v1` (stable), `v2` (employees with `FullName`).

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/auth/register` | Public | Register a new user |
| POST | `/api/v1/auth/login` | Public | Login and get JWT |
| GET | `/api/v1/employees` | Any role | Get all employees (paginated) |
| POST | `/api/v1/employees` | Admin, HR | Create employee |
| PUT | `/api/v1/employees/{id}` | Admin, HR | Update employee |
| DELETE | `/api/v1/employees/{id}` | Admin | Soft delete employee |
| GET | `/api/v2/employees` | Any role | Get employees with `FullName` field |
| GET | `/api/v1/departments` | Any role | Get all departments |
| POST | `/api/v1/departments` | Admin, HR | Create department |
| GET | `/api/v1/attendance` | Any role | Get attendance records |
| POST | `/api/v1/attendance` | Admin, HR | Create attendance record |
| GET | `/api/v1/payroll` | Any role | Get payroll records |
| POST | `/api/v1/payroll` | Admin, HR | Create payroll record |
| POST | `/api/v1/payroll/calculate` | Admin, HR | Auto-calculate payroll from attendance |
| PATCH | `/api/v1/payroll/{id}/status` | Admin, HR | Update payroll status |
| GET | `/api/v1/audit` | Admin | Get audit log (paginated, filterable) |

---

## Getting Started

### Option 1 — Docker (recommended, zero setup)

**Prerequisites:** Docker Desktop

```bash
# 1. Copy the environment file and fill in your secrets
cp .env.example .env

# 2. Start everything (builds API image, starts SQL Server, runs migrations)
docker compose up --build
```

API: `http://localhost:8080`
Swagger: `http://localhost:8080/swagger`

To stop:
```bash
docker compose down
```

> Data persists in a named Docker volume (`hrm-sqldata`). Use `docker compose down -v` to wipe it.

---

### Option 2 — Local Development

**Prerequisites:** .NET 8 SDK, SQL Server 2022

**1. Set connection string**

Update `HRM.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HRManagementSystem;..."
}
```

**2. Apply migrations**
```bash
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.API
```

**3. Run**
```bash
dotnet run --project HRM.API
```

Swagger: `https://localhost:7114/swagger`

---

## Running Tests

```bash
dotnet test
```

Run only unit tests:
```bash
dotnet test --filter "FullyQualifiedName~Integration=false"
```

Run only integration tests:
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

**Test coverage:** 130 tests total — unit tests for all CQRS handlers, EF Core interceptors, and integration tests covering the full HTTP pipeline.

---

## Environment Variables

When running with Docker, all secrets are loaded from `.env`. Never commit `.env` — use `.env.example` as a template.

| Variable | Description |
|---|---|
| `SA_PASSWORD` | SQL Server `sa` account password |
| `JWT_KEY` | JWT signing secret (min 32 characters) |
| `JWT_ISSUER` | JWT issuer claim |
| `JWT_AUDIENCE` | JWT audience claim |
| `JWT_EXPIRY_MINUTES` | Token expiry duration in minutes |

---

## Roles

| Role | Permissions |
|---|---|
| `Admin` | Full access to all endpoints |
| `HR` | Create and update employees, attendance, payroll |
| `Viewer` | Read-only access |

---

_Last updated: June 2026_
