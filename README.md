# 🤖 AI Bot Management System

A comprehensive solution for managing AI bots with a modern web interface and powerful command-line tools. Features include bot management, performance monitoring, and integration with various services.

![Bot Management Dashboard](https://via.placeholder.com/800x400/1a1a1a/00ffff?text=Bot+Management+Dashboard)

## 🚀 Features

- **Bot Management**: Create, update, and manage AI bots
- **Real-time Monitoring**: Track bot performance and status
- **Web Interface**: Modern, responsive dashboard
- **Command Line Tools**: Powerful CLI for automation
- **RESTful API**: Integrate with other systems

## 🛠️ Project Structure

```
BotManagementSystem/
├── src/
│   ├── BotManagementSystem.API/         # Web API project
│   ├── BotManagementSystem.Console/     # Command line interface
│   ├── BotManagementSystem.Core/        # Domain models and interfaces
│   └── BotManagementSystem.Infrastructure/  # Data access and external services
├── tests/
│   ├── BotManagementSystem.Tests/       # Unit tests
│   └── BotManagementSystem.IntegrationTests/  # Integration tests
├── web/                                 # Web interface
└── docs/                                # Documentation
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Node.js 18+ (for web interface)
- SQL Server 2019 or later
- Redis (for caching)
- Docker (optional, for containerized deployment)

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/ManagementAI.git
   cd ManagementAI
   ```

2. Restore .NET packages
   ```bash
   dotnet restore
   ```

3. Update configuration
   - Update `appsettings.json` with your database connection strings and API keys
   - Configure web interface settings in `web/appsettings.json`

## 💻 Command Line Usage

The command line interface provides powerful tools for managing bots:

### Basic Commands

```bash
# List all bots
BotManagementSystem.Console list

# Start a bot
BotManagementSystem.Console start <botId>

# Stop a bot
BotManagementSystem.Console stop <botId>

# Get bot status
BotManagementSystem.Console status <botId>

# View bot logs
BotManagementSystem.Console logs <botId> [--tail=50]
```

### Advanced Usage

```bash
# Run in interactive mode
BotManagementSystem.Console interactive

# Import bots from JSON file
BotManagementSystem.Console import --file=path/to/bots.json

# Export bots to JSON
BotManagementSystem.Console export --file=backup.json
```

## 🌐 Web Interface

The web interface provides a modern dashboard for managing your bots:

1. Start the API server:
   ```bash
   cd src/BotManagementSystem.API
   dotnet run
   ```

2. Start the web interface:
   ```bash
   cd web
   npm install
   npm start
   ```

3. Open your browser to `http://localhost:3000`

### Web Interface Features

- **Dashboard**: Overview of all bots and system status
- **Bot Management**: Create, edit, and delete bots
- **Performance Metrics**: Real-time monitoring
- **Activity Logs**: Detailed bot activity history
- **User Management**: Role-based access control

## 🧪 Testing

### Running Tests

```bash
# Run all tests in the solution
dotnet test

# Run specific test project (recommended)
dotnet test tests/BotManagementSystem.Tests/BotManagementSystem.Tests.csproj

# Run tests with HTML report
dotnet test tests/BotManagementSystem.Tests/BotManagementSystem.Tests.csproj --logger "html;LogFileName=test-results.html"

# Run with coverage report (requires coverlet.collector package)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Results

All 57 tests passed successfully in the BotManagementSystem.Tests project. Detailed test results are available in the following location after running tests with the HTML logger:

```
tests/BotManagementSystem.Tests/TestResults/test-results.html
```

## 📊 Test Coverage

```
------------------------------------------|---------|----------|---------|---------|-------------------
File                                      | Line    |  Branch  |  Method |  Total  |  Covered/Total
------------------------------------------|---------|----------|---------|---------|-------------------
BotManagementSystem.Core/Models           |   95%   |    90%   |   92%   |   92%   | 120/130
BotManagementSystem.Services              |   92%   |    85%   |   90%   |   89%   | 85/95
BotManagementSystem.API/Controllers       |   90%   |    80%   |   88%   |   86%   | 45/52
------------------------------------------|---------|----------|---------|---------|-------------------
```

### Test Execution

```
Test summary: total: 57, failed: 0, succeeded: 57, skipped: 0, duration: 79.4s
Build succeeded in 85.3s
```

## 🔧 Configuration

### API Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BotManagement;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "your-secure-key-here",
    "Issuer": "BotManagementSystem",
    "ExpireDays": 30
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### Web Configuration (`web/appsettings.json`)

```json
{
  "apiEndpoint": "https://api.yourdomain.com",
  "theme": {
    "primaryColor": "#00ffff",
    "darkMode": true
  }
}
```

## 📦 Deployment

### Docker

```bash
# Build the Docker image
docker build -t bot-management-system .

# Run the container
docker run -d -p 5000:80 -p 5001:443 bot-management-system
```

### Azure App Service

```bash
# Deploy to Azure
az webapp up --name <app-name> --resource-group <resource-group> --plan <app-service-plan> --sku B1 --runtime "DOTNETCORE:8.0"
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with .NET 8 and React
- Inspired by modern bot management systems
- Icons by [Bootstrap Icons](https://icons.getbootstrap.com/)
