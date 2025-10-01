# TechHive User Management API

The deployed version of the TechHive API can be viewed <a href="https://dev.dotnetmurf.net/TechHiveAPI/" target="_blank">here</a>.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

## Project Overview
This project is a modern, RESTful User Management API built with .NET 9.0 and ASP.NET Core for TechHive Solutions. It implements clean architecture principles with comprehensive authentication, validation, and testing. It allows HR and IT departments to efficiently create, update, retrieve, and delete user records.

This API was developed as an alternative version to a Mimimal API that was created as a three-part final project for the <a href="https://www.coursera.org/learn/back-end-development-with-dotnet?specialization=microsoft-full-stack-developer" target="_blank">"Back-End Development with .NET"</a>. The development process for this coursework has a focus on leveraging Microsoft Copilot for code generation, enhancement, and debugging. This course is one of twelve courses required for obtaining the <a href="https://www.coursera.org/professional-certificates/microsoft-full-stack-developer" target="_blank">"Microsoft Full-Stack Developer Professional Certificate"</a>.

## 🚀 Features

- **Clean Architecture** - 3-layer separation (Controllers → Services → Repositories)
- **Token-Based Authentication** - Custom middleware with Bearer token validation
- **In-Memory Database** - EF Core InMemory provider for rapid development
- **Auto-Seeding** - 25 sample users automatically created on startup
- **Comprehensive Validation** - Custom validators with detailed error messages
- **Global Error Handling** - Centralized exception handling middleware
- **Request/Response Logging** - Detailed logging for debugging and monitoring
- **Swagger UI** - Interactive API documentation at root path
- **Full Test Coverage** - Unit tests with Moq and integration tests with WebApplicationFactory
- **HTTP Test Files** - Ready-to-use .http files for VS Code REST Client

## 📋 Table of Contents

- [Getting Started](#getting-started)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Architecture](#architecture)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [Git](https://git-scm.com/downloads)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/dotnetmurf/TechHiveAPI.git
   cd TechHiveAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

### Running the Application

#### Development Mode
```bash
dotnet run
```

The API will start and be available at:
- **HTTP**: `http://localhost:5298`
- **HTTPS**: `https://localhost:7154` (if configured)

#### Access Swagger UI
Navigate to the root URL in your browser:
```
http://localhost:5298
```

## 📚 API Documentation

### Base URL
```
http://localhost:5298
```

### Endpoints

#### User Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | Get all users | ✅ |
| GET | `/api/users/{id}` | Get user by ID | ✅ |
| POST | `/api/users` | Create new user | ✅ |
| PUT | `/api/users/{id}` | Update existing user | ✅ |
| DELETE | `/api/users/{id}` | Delete user | ✅ |

#### Data Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/data/seed` | Reset database to 25 sample users | ✅ |

### Request/Response Examples

#### Get All Users
```http
GET /api/users HTTP/1.1
Host: localhost:5298
Authorization: Bearer TechHive2024SecureToken
```

**Response (200 OK)**
```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@techhive.com",
    "role": "Developer"
  },
  {
    "id": 2,
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@techhive.com",
    "role": "Manager"
  }
]
```

#### Create User
```http
POST /api/users HTTP/1.1
Host: localhost:5298
Authorization: Bearer TechHive2024SecureToken
Content-Type: application/json

{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@techhive.com",
  "role": "Developer"
}
```

**Response (201 Created)**
```json
{
  "id": 26,
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@techhive.com",
  "role": "Developer"
}
```

#### Update User
```http
PUT /api/users/1 HTTP/1.1
Host: localhost:5298
Authorization: Bearer TechHive2024SecureToken
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe.updated@techhive.com",
  "role": "Team Lead"
}
```

**Response (200 OK)**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe.updated@techhive.com",
  "role": "Team Lead"
}
```

### Validation Rules

#### User Fields

| Field | Rules |
|-------|-------|
| **FirstName** | Required, Max 25 characters |
| **LastName** | Required, Max 25 characters |
| **Email** | Required, Valid email format, Max 50 characters, Must be unique |
| **Role** | Required, Max 25 characters, Must be valid role |

#### Valid Roles
- Developer
- Manager
- Designer
- QA Engineer
- DevOps Engineer
- Product Manager
- Architect
- Team Lead

### Error Responses

#### 400 Bad Request - Validation Error
```json
{
  "errors": {
    "Email": ["Email is required."],
    "Role": ["Role must be one of the following: Developer, Manager, Designer, QA Engineer, DevOps Engineer, Product Manager, Architect, Team Lead"]
  }
}
```

#### 400 Bad Request - Duplicate Email
```json
{
  "error": "A user with email 'john.doe@techhive.com' already exists."
}
```

#### 401 Unauthorized
```
Unauthorized: Missing authentication token.
```

#### 404 Not Found
```
(Empty response body)
```

#### 500 Internal Server Error
```json
{
  "error": "Internal server error."
}
```

## 🔐 Authentication

The API uses **Bearer Token Authentication** via custom middleware.

### Hardcoded Token (Development)
```
TechHive2024SecureToken
```

### Using the Token

#### In Swagger UI
1. Click the **"Authorize"** button
2. Enter: `TechHive2024SecureToken`
3. Click **"Authorize"**
4. All requests will now include the token

#### In HTTP Requests
```http
Authorization: Bearer TechHive2024SecureToken
```

#### In Code (C#)
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "TechHive2024SecureToken");
```

### Bypass Authentication
Swagger endpoints (`/swagger`, `/`, `/index.html`) bypass authentication for easy access to documentation.

## 🏗️ Architecture

### Clean Architecture Pattern

```
┌─────────────────────────────────────────────────────────┐
│                    Controllers Layer                     │
│              (HTTP Requests/Responses)                   │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                   Services Layer                         │
│              (Business Logic & Validation)               │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Repositories Layer                       │
│                   (Data Access)                          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    DbContext                             │
│              (EF Core In-Memory)                         │
└─────────────────────────────────────────────────────────┘
```

### Middleware Pipeline

**⚠️ Order is critical!**

```csharp
1. ErrorHandlingMiddleware    // Catches all exceptions first
2. AuthenticationMiddleware    // Validates Bearer token
3. LoggingMiddleware          // Logs requests/responses
4. [Application Pipeline]
```

### Dependency Injection

All services are registered as `Scoped` in `Program.cs`:

```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<DataSeedService>();
```

### Data Flow Example

```
HTTP Request
    ↓
ErrorHandlingMiddleware (wraps everything)
    ↓
AuthenticationMiddleware (validates token)
    ↓
LoggingMiddleware (logs request)
    ↓
UsersController (entry point)
    ↓
UserService (business logic)
    ↓
UserRepository (data access)
    ↓
AppDbContext (EF Core)
    ↓
In-Memory Database
```

## 🧪 Testing

### Test Structure

```
TechHiveAPI.Tests/
├── Unit/
│   ├── Services/
│   │   └── UserServiceTests.cs      (10 tests)
│   └── Repositories/
│       └── UserRepositoryTests.cs   (8 tests)
└── Integration/
    └── Controllers/
        └── UsersControllerTests.cs  (10 tests)
```

### Running Tests

#### Run All Tests
```bash
dotnet test
```

#### Run Specific Test Project
```bash
dotnet test TechHiveAPI.Tests/TechHiveAPI.Tests.csproj
```

#### Run with Detailed Output
```bash
dotnet test --verbosity detailed
```

### Test Coverage

- **Unit Tests**: 18 tests - Business logic and data access
- **Integration Tests**: 10 tests - Full HTTP request/response cycle
- **Total**: 28 tests

### Unit Testing with Moq

```csharp
[Fact]
public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
{
    // Arrange
    var userCreateDto = new UserCreateDto { /* ... */ };
    _mockRepository.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
        .ReturnsAsync(false);
    
    // Act
    var result = await _userService.CreateUserAsync(userCreateDto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("John", result.FirstName);
}
```

### Integration Testing with WebApplicationFactory

```csharp
[Fact]
public async Task GetAllUsers_WithValidToken_ReturnsOkAndUsers()
{
    // Act
    var response = await _client.GetAsync("/api/users");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var users = await response.Content.ReadFromJsonAsync<List<UserReadDto>>();
    Assert.NotNull(users);
}
```

### HTTP Test Files

Located in `Tests/` folder for use with VS Code REST Client extension:

- **users-crud.http** - Complete CRUD operation examples
- **users-validation.http** - Validation testing scenarios
- **middleware-tests.http** - Authentication and middleware tests

## 📁 Project Structure

```
TechHiveAPI/
├── .github/
│   └── copilot-instructions.md      # AI development guidelines
├── Controllers/
│   ├── UsersController.cs           # User CRUD endpoints
│   └── DataController.cs            # Data management endpoints
├── Data/
│   └── AppDbContext.cs              # EF Core DbContext
├── Docs/
│   ├── BUILD_INSTRUCTIONS.md        # Build and setup guide
│   ├── SUCCESS_SUMMARY.md           # Implementation summary
│   └── GIT_COMMIT_SUMMARY.md        # Git commit history
├── DTOs/
│   ├── UserCreateDto.cs             # Create user input
│   ├── UserReadDto.cs               # User output
│   └── UserUpdateDto.cs             # Update user input
├── Middleware/
│   ├── AuthenticationMiddleware.cs  # Bearer token auth
│   ├── ErrorHandlingMiddleware.cs   # Global exception handler
│   └── LoggingMiddleware.cs         # Request/response logger
├── Models/
│   └── User.cs                      # User entity
├── Properties/
│   └── launchSettings.json          # Launch configuration
├── Repositories/
│   ├── IUserRepository.cs           # Repository interface
│   └── UserRepository.cs            # Repository implementation
├── Services/
│   ├── IUserService.cs              # Service interface
│   ├── UserService.cs               # Service implementation
│   └── DataSeedService.cs           # Database seeding
├── TechHiveAPI.Tests/
│   ├── Integration/
│   │   └── Controllers/
│   │       └── UsersControllerTests.cs
│   └── Unit/
│       ├── Repositories/
│       │   └── UserRepositoryTests.cs
│       └── Services/
│           └── UserServiceTests.cs
├── Tests/
│   ├── users-crud.http              # CRUD test requests
│   ├── users-validation.http        # Validation tests
│   └── middleware-tests.http        # Middleware tests
├── Validation/
│   └── ValidRoleAttribute.cs        # Custom role validator
├── Directory.Build.props            # Build configuration
├── Program.cs                       # Application entry point
├── TechHiveAPI.csproj              # Project file
├── TechHiveAPI.sln                 # Solution file
└── README.md                        # This file
```

## ⚙️ Configuration

### Database Configuration

The application uses **EF Core In-Memory Database** configured in `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));
```

### Logging Configuration

Logging is configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Swagger Configuration

- **Root Path**: Swagger UI is available at the application root (`/`)
- **XML Documentation**: Enabled for all public APIs
- **Bearer Auth**: Integrated authentication UI

## 🗄️ Database

### In-Memory Database

- **Provider**: Microsoft.EntityFrameworkCore.InMemory
- **Database Name**: `UserDb`
- **Persistence**: Data resets on application restart
- **Auto-Seeding**: 25 sample users created on startup

### Sample Users

The application seeds 25 users with various roles:
- 8 Developers
- 3 Managers
- 2 Designers
- 2 QA Engineers
- 2 DevOps Engineers
- 2 Product Managers
- 2 Architects
- 2 Team Leads

### Resetting Data

To reset the database to the original 25 users:

```http
POST /api/data/seed
Authorization: Bearer TechHive2024SecureToken
```

## 🔧 Development

### Adding New Features

Follow the clean architecture pattern:

1. **Add DTO** in `DTOs/` folder
2. **Update Service Interface** in `Services/`
3. **Implement Service Logic** in `Services/`
4. **Update Repository** if needed in `Repositories/`
5. **Add Controller Action** in `Controllers/`
6. **Add Tests** in `TechHiveAPI.Tests/`
7. **Create HTTP Test** in `Tests/`

### Code Style

- Use C# 12 features (`.NET 9.0`)
- Follow clean architecture principles
- Add XML documentation for all public APIs
- Write unit tests for business logic
- Write integration tests for endpoints

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **dotnetmurf** - *Initial work* - [GitHub](https://github.com/dotnetmurf)

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- Testing with [xUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq4)
- Documentation with [Swagger/OpenAPI](https://swagger.io/)

## 📞 Support

For issues, questions, or contributions, please open an issue on the [GitHub repository](https://github.com/dotnetmurf/TechHiveAPI/issues).

---

**Built with ❤️ using .NET 9.0**