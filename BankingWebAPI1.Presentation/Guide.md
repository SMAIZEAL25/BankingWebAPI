## 2. GUIDE.md


# BankingWebAPI - Developer Guide

This guide provides a detailed overview of the project's architecture, code structure, and API endpoints.

## Architecture & Code Structure

The solution is structured using Clean Architecture and CQRS, promoting separation of concerns and testability.

### Project Layers:

1.  **Domain (`Banking.Domain`)**
    - **Entities**: `Account`, `Transaction`, `User`, etc.
    - **Value Objects**: `Money`, `Email`.
    - **Enums**: `TransactionType`, `TransactionStatus`, `Role`.
    - **Exceptions**: Custom domain exceptions (e.g., `InsufficientFundsException`).
    - **Interfaces**: Core repository interfaces (e.g., `IAccountRepository`).

2.  **Application (`Banking.Application`)**
    - **Features**: Organized by vertical slice architecture (e.g., `Accounts`, `Transactions`, `Auth`).
        - `Commands`: CreateAccount, DepositFunds, TransferFunds, etc., and their handlers.
        - `Queries`: GetAccountDetails, GetTransactionHistory, etc., and their handlers.
    - **Behaviors**: Pipeline behaviors for logging, validation, and performance tracking.
    - **Models**: DTOs (Data Transfer Objects) for requests and responses.
    - **Interfaces**: For external services (e.g., `IPaymentService`).

3.  **Infrastructure (`Banking.Infrastructure`)**
    - **Persistence**: Entity Framework Core configurations, `ApplicationDbContext`, repository implementations.
    - **Services**: Concrete implementations of external services (e.g., `PaystackPaymentService`).
    - **Caching**: `RedisCacheService` implementation.
    - **Identity**: JWT token generation and role management.

4.  **WebAPI (`Banking.WebAPI`)**
    - **Controllers**: Thin controllers that dispatch MediatR requests.
    - **Middleware**: Exception handling, request logging.
    - **Dependency Injection**: Registration of services from all layers.
    - **Configuration**: `appsettings.json`, environment variables.

### Key Patterns & Libraries:

- **CQRS**: Using MediatR to separate read (Queries) and write (Commands) operations.
- **Repository Pattern**: Abstracting data access behind interfaces.
- **FluentValidation**: Used in the Application layer for validating Commands.
- **Entity Framework Core**: ORM for data persistence with MySQL.
- **AutoMapper**: For mapping between Entities and DTOs.

## API Endpoints

All endpoints are prefixed with `/api`. JWT Authentication is required for all endpoints unless specified.

### Authentication (`/api/auth`)

| Method | Endpoint       | Description | Authorization |
| :----- | :------------- | :-------------------------------- | :-------------------------- |
| `POST` | `/register`    | Registers a new user.             | Public                      |
| `POST` | `/login`       | Authenticates a user and returns a JWT token. | Public |
| `POST` | `/assign-role` | Assigns a role to a user (e.g., Admin, User). | Admin |

### Accounts (`/api/accounts`)

| Method | Endpoint       | Description | Authorization |
| :----- | :------------- | :-------------------------------- | :-------------------------- |
| `POST` | `/`            | Creates a new bank account.       | User, Admin                 |
| `GET`  | `/`            | Gets a list of all accounts.      | Admin                       |
| `GET`  | `/{accountId}` | Gets details for a specific account. | User (own account), Admin |
| `GET`  | `/{accountId}/transactions` | Gets transaction history for an account. | User (own account), Admin |

### Transactions (`/api/transactions`)

| Method | Endpoint       | Description | Authorization |
| :----- | :------------- | :-------------------------------- | :-------------------------- |
| `POST` | `/deposit`     | Deposits funds into an account.   | User, Admin                 |
| `POST` | `/withdraw`    | Withdraws funds from an account.  | User, Admin                 |
| `POST` | `/transfer`    | Transfers funds between accounts. | User, Admin                 |

### Payments (`/api/payments`) - (Bonus Feature)

| Method | Endpoint            | Description | Authorization |
| :----- | :------------------ | :-------------------------------- | :-------------------------- |
| `POST` | `/initialize`       | Initializes a Paystack payment for deposit. | User, Admin |
| `GET`  | `/verify/{reference}`| Verifies and confirms a Paystack transaction. | User, Admin |

### Health & Monitoring (`/api`)

| Method | Endpoint       | Description | Authorization |
| :----- | :------------- | :-------------------------------- | :-------------------------- |
| `GET`  | `/health`      | Basic health check of the API.    | Public                      |

## Example Flow: Account Transfer

1.  **Client** sends a `POST` request to `/api/transactions/transfer` with a valid JWT token and payload:
    ```json
    {
      "FromAccountId": "a90d8510-...",
      "ToAccountId": "b82e8511-...",
      "Amount": 150.00,
      "Narration": "Lunch money"
    }
    ```
2.  **TransferCommand** is created and handled by `TransferCommandHandler`.
3.  **Handler** uses the `IAccountRepository` to fetch both accounts within a database transaction (ensuring atomicity).
4.  **Domain Logic** checks for sufficient funds in `FromAccount`. Throws `InsufficientFundsException` if invalid.
5.  **EF Core** updates the account balances and creates two `Transaction` records (Debit & Credit).
6.  **Cache** for both accounts is invalidated in Redis.
7.  **Response** is returned to the client confirming the successful transfer.

## Testing Strategy

- **Unit Tests**: Test individual commands, queries, and validation rules in isolation (`Application.UnitTests`).
- **Integration Tests**: Test the full flow from API controller to database (`WebAPI.IntegrationTests`). Uses a separate test database.
- **Mocking**: External services (Paystack) and repositories are mocked using `Moq` where appropriate.

## Configuration & Secrets

- Connection strings, API keys, and other settings are read from `appsettings.json` or environment variables.
- **Never commit secrets** to the repository. Use the Secret Manager tool for local development or environment variables in production.
    ```bash
    dotnet user-secrets set "Paystack:SecretKey" "sk_test_..."
    ```

## Logging

Structured logging is implemented using Serilog. Logs are written to the Console and a file (`logs/log-.txt`). Key information like HTTP requests, commands executed, and errors are logged for monitoring and debugging.