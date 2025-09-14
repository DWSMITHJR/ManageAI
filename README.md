# AI Bot Management System

A comprehensive solution for managing AI bots with integrations to various services including Google, Azure, and SmartThings.

## Project Structure

```
BotManagementSystem/
├── src/
│   ├── BotManagementSystem.API/         # Web API project
│   ├── BotManagementSystem.Core/        # Domain models and interfaces
│   ├── BotManagementSystem.Infrastructure/  # Data access and external services
│   └── BotManagementSystem.Services/    # Business logic and services
├── tests/
│   ├── BotManagementSystem.UnitTests/   # Unit tests
│   └── BotManagementSystem.IntegrationTests/  # Integration tests
└── docs/                                # Documentation
```

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later
- Redis
- Docker (optional, for containerized deployment)

## Getting Started

1. Clone the repository
2. Update connection strings in `appsettings.json`
3. Run database migrations
4. Start the application

## Configuration

Update the following in `appsettings.json`:
- Database connection strings
- JWT settings
- External service credentials (Google, Azure, SmartThings)

## Running the Application

```bash
dotnet run --project src/BotManagementSystem.API
```

## Testing

Run unit tests:
```bash
dotnet test
```

## License

MIT
