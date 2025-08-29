# Banking Transaction System API

A robust, secure, and scalable backend banking API built with .NET 8, following Clean Architecture and CQRS patterns. This system handles core banking operations, integrates with external payment providers, and ensures data integrity and security.

## Features

- **Account Management**: Create accounts, view details, balances, and transaction history.
- **Transaction Processing**: Secure deposits, withdrawals, and internal account transfers with atomicity and consistency.
- **External Payment Integration**: Seamless integration with Paystack for online deposits and withdrawals.
- **Security**: Role-Based Access Control (RBAC), JWT Authentication, and encryption of sensitive data.
- **Performance**: Caching with Redis and optimized database queries for high concurrency.
- **Monitoring & Logging**: Structured logging with Serilog and health checks.
- **Testing**: Comprehensive unit and integration tests with xUnit.

## Tech Stack

- **Framework**: .NET 8
- **Database**: SQL Server with Entity Framework Core
- **CQRS & Mediator**: MediatR with FluentValidation
- **Caching**: Redis
- **Authentication**: JWT Bearer Tokens
- **External API**: Paystack Integration
- **Logging**: Serilog
- **Documentation**: Swagger

## Prerequisites

- .NET 8 SDK
- MySQL Server
- Redis Server
- A [Paystack](https://paystack.com/) account for test keys

##  Setup & Installation

1.  **Clone the Repository**
    ```bash
    git clone https://github.com/SMAIZEAL25/BankingWebAPI.git
    cd BankingWebAPI
    ```

2.  **Configure the Database**
    - Create a MySQL database named `BankingDb`.
    - Update the connection string in `appsettings.json` or use environment variables.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=BankingDb;Uid=your_user;Pwd=your_password;"
    }
    ```

3.  **Apply Database Migrations**
    Run the following commands in the project directory:
    ```bash
    dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
    ```
    This will create the necessary tables.

4.  **Configure Redis (Optional for Caching)**
    - Ensure Redis server is running on `localhost:6379` or update the configuration in `appsettings.json`.
    ```json
    "Redis": {
      "Configuration": "localhost:6379",
      "InstanceName": "BankingAPI_"
    }
    ```

5.  **Configure Paystack (Optional for Payments)**
    - Add your Paystack test Secret Key to the configuration.
    ```json
    "Paystack": {
      "BaseUrl": "https://api.paystack.co",
      "SecretKey": "sk_test_your_secret_key_here"
    }
    ```

6.  **Run the Application**
    ```bash
    cd src/WebAPI
    dotnet run
    ```
    The API will be available at `https://localhost:7000` (or `http://localhost:5000`). Swagger documentation will be available at `/swagger`.

## Default Roles & Testing

Upon first run, the system seeds two default roles:
- **Admin**: Full access to all endpoints.
- **User**: Access to own account and transaction operations.

Use the `Auth/register` and `Auth/login` endpoints to create users and obtain JWT tokens. Use the token in the `Authorization` header (Bearer scheme) to access protected endpoints.

##  Running Tests

Execute the following command in the root directory to run the test suite:
```bash
dotnet test
