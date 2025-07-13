# SurveyBasket 📊

A modern, feature-rich survey management system built with ASP.NET Core 9.0 that enables organizations to create, manage, and analyze surveys with advanced authentication, role-based permissions, and real-time notifications.

## 🚀 Features

### Core Functionality

- **Poll/Survey Management**: Create, update, delete, and manage surveys with flexible question types
- **User Management**: Comprehensive user registration, authentication, and profile management
- **Role-Based Access Control**: Fine-grained permissions system with customizable roles
- **Voting System**: Secure voting mechanism with answer tracking and validation
- **Results & Analytics**: Real-time survey results with comprehensive reporting

### Authentication & Security

- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Role-Based Authorization**: Multi-level permission system (Admin, Member, etc.)
- **Email Verification**: Account confirmation via email
- **Password Security**: Configurable password policies and secure storage

### Background Processing

- **Hangfire Integration**: Background job processing for notifications and scheduled tasks
- **Email Notifications**: Automated email notifications for new polls and updates
- **Recurring Jobs**: Scheduled notifications and maintenance tasks

### API & Documentation

- **RESTful API**: Clean, well-documented API endpoints
- **Swagger Integration**: Interactive API documentation
- **Health Checks**: Comprehensive health monitoring for database and external services

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: ASP.NET Core Identity with JWT Bearer tokens
- **Background Jobs**: Hangfire
- **Email**: MailKit
- **Mapping**: Mapster
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Caching**: HybridCache
- **Health Checks**: ASP.NET Core Health Checks

## 📋 Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Git

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ziadhanii/Survey-Basket-API.git
cd SurveyBasket.Api
```

### 2. Database Setup

1. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SurveyBasket;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True",
    "HangfireConnection": "Server=.;Database=SurveyBasketJobs;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True"
  }
}
```

2. Run Entity Framework migrations:

```bash
cd SurveyBasket.Api
dotnet ef database update
```

### 3. Configure User Secrets

Set up user secrets for sensitive configuration:

```bash
dotnet user-secrets init
dotnet user-secrets set "JwtOptions:Key" "your-super-secret-key-here-minimum-32-characters"
dotnet user-secrets set "JwtOptions:Issuer" "SurveyBasket"
dotnet user-secrets set "JwtOptions:Audience" "SurveyBasket-Users"
dotnet user-secrets set "MailSettings:Host" "your-smtp-host"
dotnet user-secrets set "MailSettings:Port" "587"
dotnet user-secrets set "MailSettings:Mail" "your-email@domain.com"
dotnet user-secrets set "MailSettings:Password" "your-email-password"
```

### 4. Run the Application

```bash
dotnet run
```

The application will be available at:

- **API**: `https://localhost:5001` (HTTPS) or `http://localhost:5001` (HTTP)
- **Swagger UI**: `https://localhost:5001` (root path)
- **Hangfire Dashboard**: `https://localhost:5001/jobs`

## 📁 Project Structure

```
SurveyBasket.Api/
├── Controllers/          # API Controllers
│   ├── AuthController.cs        # Authentication endpoints
│   ├── PollsController.cs       # Poll management
│   ├── QuestionsController.cs   # Question management
│   ├── UsersController.cs       # User management
│   ├── RolesController.cs       # Role management
│   ├── VotesController.cs       # Voting system
│   └── ResultsController.cs     # Results & analytics
├── Services/             # Business Logic Services
├── Entities/             # Database Models
├── Contracts/            # DTOs and Request/Response models
├── Authentication/       # JWT and Auth configuration
├── Persistence/          # Entity Framework configuration
├── Errors/               # Error handling and custom exceptions
├── Extensions/           # Extension methods
├── Health/               # Health check implementations
├── Mapping/              # Object mapping configuration
└── Settings/             # Configuration models
```

## 🔧 Configuration

### Key Configuration Sections

#### JWT Settings

```json
{
  "JwtOptions": {
    "Key": "your-secret-key",
    "Issuer": "SurveyBasket",
    "Audience": "SurveyBasket-Users",
    "ExpiryMinutes": 60
  }
}
```

#### Email Settings

```json
{
  "MailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Mail": "your-email@domain.com",
    "Password": "your-app-password",
    "DisplayName": "Survey Basket"
  }
}
```

#### Hangfire Settings

```json
{
  "HangfireSettings": {
    "Username": "admin",
    "Password": "your-hangfire-password"
  }
}
```

## 🔐 API Endpoints

### Authentication

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/confirm-email` - Email confirmation
- `POST /api/auth/forgot-password` - Password reset request
- `POST /api/auth/reset-password` - Password reset

### Polls

- `GET /api/polls` - Get all polls (Admin)
- `GET /api/polls/current` - Get current active polls
- `GET /api/polls/{id}` - Get specific poll
- `POST /api/polls` - Create new poll
- `PUT /api/polls/{id}` - Update poll
- `DELETE /api/polls/{id}` - Delete poll

### Questions

- `GET /api/questions` - Get questions for a poll
- `POST /api/questions` - Add question to poll
- `PUT /api/questions/{id}` - Update question
- `DELETE /api/questions/{id}` - Delete question

### Votes

- `POST /api/votes` - Submit vote
- `GET /api/votes/user/{pollId}` - Get user's vote for a poll

### Results

- `GET /api/results/{pollId}` - Get poll results

## 🏗️ Architecture

The application follows Clean Architecture principles with:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Implement business logic
- **Entities**: Define domain models
- **Persistence**: Data access layer with Entity Framework
- **Authentication**: JWT-based security with role permissions
- **Background Jobs**: Hangfire for async processing

## 🧪 Development

### Adding Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Running Tests

```bash
dotnet test
```

### Code Style

The project uses EditorConfig for consistent code formatting. Make sure your IDE respects the `.editorconfig` file.

## 🚀 Deployment

### Docker (Optional)

Create a `Dockerfile` for containerized deployment:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SurveyBasket.Api/SurveyBasket.Api.csproj", "SurveyBasket.Api/"]
RUN dotnet restore "SurveyBasket.Api/SurveyBasket.Api.csproj"
COPY . .
WORKDIR "/src/SurveyBasket.Api"
RUN dotnet build "SurveyBasket.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SurveyBasket.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SurveyBasket.Api.dll"]
```

### Production Considerations

- Configure proper connection strings for production database
- Set up SSL certificates
- Configure proper email settings
- Set strong JWT secret keys
- Enable logging to files in production
- Configure health checks monitoring
- Set up proper CORS policies

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:

- Create an issue in the [GitHub repository](https://github.com/ziadhanii/Survey-Basket-API/issues)
- Check the [Wiki](https://github.com/ziadhanii/Survey-Basket-API/wiki) for detailed documentation
- Review the Swagger documentation at the root URL when running the application

## 🔄 Changelog

### Version 1.0.0

- Initial release
- Core survey management functionality
- JWT authentication and authorization
- Background job processing
- Email notifications
- Comprehensive API documentation

## 👤 Author

### Ziad Hani

- GitHub: [@ziadhanii](https://github.com/ziadhanii)
- Repository: [Survey-Basket-API](https://github.com/ziadhanii/Survey-Basket-API)

---

## ⭐ Show Your Support

If this project helped you, please consider giving it a star on GitHub!

[⭐ Star this repository](https://github.com/ziadhanii/Survey-Basket-API)

---
