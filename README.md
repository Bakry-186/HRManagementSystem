# HR Management System

A production-quality HR Management System built with ASP.NET Core Web API following Clean Architecture principles.

## Architecture Overview

The solution is structured into four distinct layers. Each layer has a single responsibility and a strict dependency rule: inner layers never depend on outer layers.

```
HRManagementSystem.sln
├── HRM.Domain
├── HRM.Application
├── HRM.Infrastructure
└── HRM.API
```

### HRM.Domain

The core of the system. Contains enterprise business entities, enums, and domain exceptions. This layer has **zero dependencies** on any other layer or external library. It represents the business concepts that exist regardless of technology — Employee, Department, etc.

### HRM.Application

Orchestrates the business use cases. Contains DTOs, service interfaces, repository interfaces, AutoMapper profiles, and validators. It depends only on `HRM.Domain`. It defines **what** the system can do without caring about **how** it is done (no EF Core, no HTTP, no SQL here).

### HRM.Infrastructure

Implements the interfaces defined in `HRM.Application`. Contains the EF Core `DbContext`, repository implementations, database migrations, and any external service integrations. It knows about SQL Server and Entity Framework. Nothing outside this layer should.

### HRM.API

The entry point. Contains Controllers, middleware, and the dependency injection wiring (`Program.cs`). It receives HTTP requests, delegates to the Application layer, and returns responses. No business logic lives here.

## Dependency Flow

```
HRM.API  ──►  HRM.Application  ──►  HRM.Domain
   │                                     ▲
   └──────►  HRM.Infrastructure  ────────┘
```

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 8) |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| Documentation | Swagger / OpenAPI 3.0 |
| Containerization | Docker + Docker Compose |

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server 2022
- Docker (optional)

### Run locally

```bash
cd HRM.API
dotnet run
```

Swagger UI available at: `https://localhost:7114/swagger`

### Apply database migrations

```bash
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.API
```
