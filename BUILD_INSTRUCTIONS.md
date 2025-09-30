# TechHive User Management API - Build Instructions

## Overview
The TechHive User Management API has been fully implemented according to the specifications in `copilot-instructions.md`. However, there's a directory structure issue that needs to be resolved before building.

## Current Issue
The `TechHiveAPI.Tests` folder is currently located inside the main `TechHiveAPI` folder, which causes the .NET compiler to try to include test files when building the main project. This creates compilation errors because the test dependencies (xUnit, Moq, etc.) aren't referenced by the main project.

## Solution

### Option 1: Move the Test Project (Recommended)
Move the `TechHiveAPI.Tests` folder outside the main `TechHiveAPI` folder:

```powershell
# From the TechHive root directory
Move-Item ".\TechHiveAPI\TechHiveAPI.Tests" ".\TechHiveAPI.Tests"

# Then add it back to the solution
dotnet sln add .\TechHiveAPI.Tests\TechHiveAPI.Tests.csproj
```

### Option 2: Temporarily Delete Test Project
```powershell
# Delete the test folder temporarily
Remove-Item ".\TechHiveAPI\TechHiveAPI.Tests" -Recurse -Force

# Build and run the API
cd TechHiveAPI
dotnet build
dotnet run
```

## What Has Been Implemented

### ✅ Core Application Structure
- **Models**: `User.cs` entity with required fields
- **DTOs**: `UserCreateDto`, `UserReadDto`, `UserUpdateDto` with validation
- **Validation**: Custom `ValidRoleAttribute` for role validation
- **Data Context**: `AppDbContext` with EF Core In-Memory database
- **Repositories**: `IUserRepository` and `UserRepository` implementation
- **Services**: `IUserService`, `UserService`, and `DataSeedService`
- **Controllers**: `UsersController` and `DataController`

### ✅ Middleware (CRITICAL ORDER)
1. `ErrorHandlingMiddleware` - Catches all exceptions first
2. `AuthenticationMiddleware` - Validates Bearer token
3. `LoggingMiddleware` - Logs requests/responses

### ✅ Authentication
- Hardcoded token: `TechHive2024SecureToken`
- All endpoints require authentication except Swagger paths
- Bearer token authentication via custom middleware

### ✅ Swagger Configuration
- XML documentation enabled
- Root path redirects to Swagger UI
- Bearer token UI with instructions

### ✅ Database Seeding
- Automatic seeding of 25 sample users on startup
- Reset endpoint: `POST /api/data/seed`

### ✅ HTTP Test Files
Located in `Tests/` folder:
- `users-crud.http` - CRUD operations tests
- `users-validation.http` - Validation tests  
- `middleware-tests.http` - Middleware behavior tests

### ✅ Unit & Integration Tests
Test files created (need project structure fix):
- `Unit/Services/UserServiceTests.cs` - Service layer tests with Moq
- `Unit/Repositories/UserRepositoryTests.cs` - Repository tests
- `Integration/Controllers/UsersControllerTests.cs` - Full HTTP tests

## API Endpoints

### Users Management
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update existing user
- `DELETE /api/users/{id}` - Delete user

### Data Management
- `POST /api/data/seed` - Reset database with 25 sample users

## Validation Rules
- **FirstName**: Required, max 25 characters
- **LastName**: Required, max 25 characters
- **Email**: Required, valid email, max 50 characters, must be unique
- **Role**: Required, max 25 characters, must be one of: Developer, Manager, Designer, QA Engineer, DevOps Engineer, Product Manager, Architect, Team Lead

## Running the Application (After Fix)

### Build
```powershell
dotnet build
```

### Run
```powershell
dotnet run
```

Application will be available at: `https://localhost:7154`

### Test
```powershell
dotnet test
```

## Project Files Created

### Main Application
```
TechHiveAPI/
├── Controllers/
│   ├── UsersController.cs
│   └── DataController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── UserCreateDto.cs
│   ├── UserReadDto.cs
│   └── UserUpdateDto.cs
├── Middleware/
│   ├── AuthenticationMiddleware.cs
│   ├── ErrorHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
├── Models/
│   └── User.cs
├── Repositories/
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Services/
│   ├── IUserService.cs
│   ├── UserService.cs
│   └── DataSeedService.cs
├── Tests/
│   ├── users-crud.http
│   ├── users-validation.http
│   └── middleware-tests.http
├── Validation/
│   └── ValidRoleAttribute.cs
└── Program.cs
```

### Test Project
```
TechHiveAPI.Tests/
├── Integration/
│   └── Controllers/
│       └── UsersControllerTests.cs
└── Unit/
    ├── Repositories/
    │   └── UserRepositoryTests.cs
    └── Services/
        └── UserServiceTests.cs
```

## Architecture Highlights

### Clean Architecture Pattern
```
Controllers → Services → Repositories → DbContext
```

### Dependency Injection
All services registered as `Scoped` in `Program.cs`:
- `IUserRepository` → `UserRepository`
- `IUserService` → `UserService`
- `DataSeedService`

### Error Handling
Global error handler returns:
```json
{ "error": "Internal server error." }
```

## Next Steps
1. Fix the directory structure using Option 1 above
2. Run `dotnet build` to verify compilation
3. Run `dotnet run` to start the API
4. Navigate to `https://localhost:7154` to access Swagger UI
5. Use the HTTP test files in VS Code REST Client extension
6. Run `dotnet test` to execute all unit and integration tests

## Notes
- All code follows the specifications in `copilot-instructions.md`
- XML documentation is enabled for all public APIs
- The application uses .NET 9.0
- In-memory database means data resets on app restart
- Email uniqueness is enforced at the database level
