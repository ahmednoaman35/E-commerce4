# E-Commerce API

A RESTful E-Commerce backend built with **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**. It covers authentication with JWT, product browsing with filtering and pagination, orders, and payments, and it includes unit tests and manual API test documentation.

## Features

- **Authentication and authorization**
  - Register and login with ASP.NET Core Identity
  - JWT access tokens with role claims
  - Seeded `Admin` and `SuperAdmin` roles and demo accounts
  - Check if an email exists, get the current user, get and update the user address
- **Products**
  - List products with filtering by brand and type, search by name, and pagination
  - Get a product by id (returns 404 when it does not exist)
  - List product brands and types
- **Orders and payments**
  - Create and view orders and process payments
- **API documentation** with Swagger / OpenAPI

## Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core Web API |
| ORM and database | Entity Framework Core, SQL Server |
| Identity and security | ASP.NET Core Identity, JWT |
| Mapping | AutoMapper |
| Testing | xUnit, Moq, Postman |
| API docs | Swagger / OpenAPI |

## Architecture

The solution follows a layered (clean) architecture with the Repository, Unit of Work, and Specification patterns:

| Layer | Responsibility |
|---|---|
| `Domain` | Entities, contracts (interfaces), and custom exceptions |
| `Services` | Business logic (for example `ProductService`, `AuthenticationService`) |
| `Infrastructure` (`Persistence`) | `DbContext`, repositories, Unit of Work, migrations, and data seeding |
| `Shared` | DTOs and options classes |
| `E-Commerce.API` | Controllers, middleware, and application startup |
| `E-Commerce.Tests` | Unit tests |

The services depend on interfaces such as `IUnitOfWork` and `IGenericRepository`, so they can be tested in isolation with mocks and without a real database.

## Getting Started

### Prerequisites

- .NET SDK (the version used by the solution)
- SQL Server (LocalDB, Express, or a full instance)
- Visual Studio 2022 or the `dotnet` CLI

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/ahmednoaman35/E-commerce4.git
   cd E-commerce4
   ```
2. Set your SQL Server connection string in `E-Commerce.API/appsettings.json`.
3. Configure the JWT settings (`JwtOptions`). Do not commit a real secret key. Use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables:
   ```bash
   dotnet user-secrets set "JwtOptions:SecretKey" "<a-long-random-secret>"
   ```
4. Create the database. In Package Manager Console, set the default project to `Infrastructure` and the startup project to `E-Commerce.API`, then run:
   ```
   Update-Database
   ```
5. Run the API and open Swagger at `https://localhost:5001/swagger`.

### Demo accounts

The database initializer seeds two accounts for **local development only**. Change or remove them before any real deployment.

| Role | Email | Password |
|---|---|---|
| SuperAdmin | `SuperAdmin@gmail.com` | `Passw0rd` |
| Admin | `Admin@gmail.com` | `Passw0rd` |

## Testing

### Unit tests

The `E-Commerce.Tests` project uses **xUnit** and **Moq** with the Arrange / Act / Assert pattern. Run them from Visual Studio (Test Explorer, `Ctrl+E, T`) or with:

```bash
dotnet test
```

| Service | Test | Type |
|---|---|---|
| ProductService | Product not found throws `ProductNotFoundException` | Negative |
| ProductService | Product exists returns the mapped DTO | Positive |
| ProductService | Pagination returns correct `PageSize` and `TotalCount` | Positive |
| ProductService | No products returns an empty paginated result | Boundary |
| AuthenticationService | Login with unregistered email throws `UnAuthorizedException` | Negative |
| AuthenticationService | Login with wrong password throws `UnAuthorizedException` | Negative |
| AuthenticationService | Login with valid credentials returns user and JWT | Positive |
| AuthenticationService | Register failure throws `ValidationException` | Negative |
| AuthenticationService | Register success returns user and JWT | Positive |

### Manual and API testing

Manual test cases, bug reports, and a Postman collection are kept in [`docs/testing`](docs/testing):

- `TestCases` : test cases for Authentication, Products, Orders, and Payments (ID, title, preconditions, steps, expected result, actual result, status)
- `BugReports.md` : defects found during testing, with reproduction steps, severity, and expected versus actual results
- `E-Commerce.postman_collection.json` : Postman collection with request tests, runnable with the Collection Runner for regression checks

## Author

**Ahmed Mohamed Noaman** : Backend developer (ASP.NET Core)

[GitHub](https://github.com/ahmednoaman35) · [LinkedIn](https://linkedin.com/in/ahmed-noaman-3528a9402)
