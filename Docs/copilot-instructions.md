# TechHive User Management API - AI Agent Instructions

## Project Overview

This is a .NET 9.0 ASP.NET Core Web API implementing a user management system with clean architecture principles. The project uses an **in-memory database** (EF Core InMemory) and **token-based authentication** with a hardcoded token for demonstration purposes.

## Architecture

**3-Layer Clean Architecture:**
```
Controllers (HTTP) → Services (Business Logic) → Repositories (Data Access) → DbContext
```

**Critical Pattern:** All layers use dependency injection with interface-based abstractions:
- `IUserService` → `UserService`
- `IUserRepository` → `UserRepository`
- Services are registered as `Scoped` in `Program.cs`

**Middleware Pipeline Order (CRITICAL):**
```csharp
// Program.cs - Order matters!
app.UseMiddleware<ErrorHandlingMiddleware>();      // 1. Catches all exceptions first
app.UseMiddleware<AuthenticationMiddleware>();     // 2. Validates token
app.UseMiddleware<LoggingMiddleware>();            // 3. Logs request/response
```

## Authentication & Security

**Hardcoded Token:** `TechHive2024SecureToken` (see `AuthenticationMiddleware.cs`)
- **ALL endpoints require authentication** except Swagger paths (`/swagger`, `/`, `/index.html`)
- Token validation happens in custom middleware, NOT using ASP.NET Core Identity
- Authorization header format: `Bearer TechHive2024SecureToken`

## Key Development Workflows

### Running the API
```powershell
dotnet run                                    # Runs on https://localhost:7154
dotnet build                                  # Build only
dotnet test                                   # Run all tests (xUnit)
```

### Testing Options
1. **Swagger UI:** Navigate to `https://localhost:7154` (root path redirects to Swagger)
2. **HTTP Files:** Use VS Code REST Client with `Tests/*.http` files
   - `users-crud.http` - CRUD operations
   - `users-validation.http` - Validation tests
   - `middleware-tests.http` - Middleware behavior tests

### Test Structure
```
TechHiveAPI.Tests/
├── Unit/                    # Moq-based unit tests
│   ├── Services/           # Test service business logic
│   └── Repositories/       # Test repository data access
└── Integration/            # WebApplicationFactory tests
    └── Controllers/        # Full HTTP request/response cycle tests
```

**Integration Test Pattern:** Uses `WebApplicationFactory<Program>` with `IClassFixture` for testing with in-memory database and real middleware pipeline.

## Project-Specific Conventions

### DTOs vs Models
- **Models/User.cs:** Entity Framework entity with navigation properties
- **DTOs/UserCreateDto.cs:** Input validation with `[Required]`, `[EmailAddress]`, `[StringLength]`, and `[ValidRole]` attributes
- **DTOs/UserReadDto.cs:** Output shape (excludes sensitive fields if any)
- **DTOs/UserUpdateDto.cs:** Update validation (all fields required)

**Mapping:** Manual mapping in `UserService.cs` using private `MapToReadDto()` method (no AutoMapper)

### Validation Rules
- **FirstName:** Required, max 25 characters
- **LastName:** Required, max 25 characters  
- **Email:** Required, valid email format, max 50 characters
- **Role:** Required, max 25 characters, must be one of: "Developer", "Manager", "Designer", "QA Engineer", "DevOps Engineer", "Product Manager", "Architect", "Team Lead"
- Custom validation attribute: `ValidRoleAttribute` in `Validation/` folder

### Error Handling Pattern
```csharp
// Service Layer: Log and re-throw
try {
    var users = await _userRepository.GetAllUsersAsync();
    return users.Select(MapToReadDto);
}
catch (Exception ex) {
    _logger.LogError(ex, "Error occurred while retrieving all users");
    throw; // Re-throw, let middleware handle HTTP response
}
```

**Global Error Handler:** `ErrorHandlingMiddleware` catches all exceptions and returns:
```json
{ "error": "Internal server error." }
```

### Logging Strategy
- **LoggingMiddleware:** Logs ALL requests/responses with timestamps and duration
- **Controllers:** Log at Info level for entry points
- **Services:** Log at Error level for exceptions, Debug/Info for business events
- Uses built-in `ILogger<T>` (not Serilog in this project)

### Swagger Configuration
- **Root path IS Swagger UI** (`options.RoutePrefix = string.Empty`)
- XML documentation enabled (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
- Bearer token UI configured with description: "Enter 'TechHive2024SecureToken' in the text input below"

## Database Considerations

**In-Memory Database Name:** `"UserDb"` (configured in `Program.cs`)
- Data resets on app restart
- No migrations needed
- Email uniqueness enforced via `HasIndex(u => u.Email).IsUnique()` in `AppDbContext.OnModelCreating()`
- **Auto-seeding:** 25 sample users are automatically created at startup via `DataSeedService`
- **Reset endpoint:** `POST /api/data/seed` clears and recreates the 25 sample users

## Adding New Endpoints

1. **Create DTOs** in `DTOs/` folder
2. **Add Service Interface** method in `Services/IUserService.cs`
3. **Implement Service** business logic in `Services/UserService.cs`
4. **Add Repository Interface** method if needed in `Repositories/IUserRepository.cs`
5. **Implement Repository** data access in `Repositories/UserRepository.cs`
6. **Add Controller Action** in `Controllers/UsersController.cs` with:
   - XML summary comments
   - `[ProducesResponseType]` attributes for all status codes
   - Proper `ActionResult<T>` return types
7. **Create HTTP Test File** in `Tests/` folder
8. **Write Unit Tests** in `TechHiveAPI.Tests/Unit/`
9. **Write Integration Tests** in `TechHiveAPI.Tests/Integration/`

## Common Pitfalls

- **Middleware order:** ErrorHandling MUST be first, Authentication before Logging
- **Swagger auth bypass:** Check `context.Request.Path.StartsWithSegments("/swagger")` in any new middleware
- **In-memory DB state:** Database is fresh on each app start; tests should create their own test data
- **Async all the way:** All data operations use `async/await` pattern consistently
- **Null checking:** Constructor injection uses `?? throw new ArgumentNullException(nameof(param))`

## Reference Files

Key architectural examples:
- **Middleware Pattern:** `Middleware/AuthenticationMiddleware.cs`
- **Service Pattern:** `Services/UserService.cs`
- **Data Seeding Service:** `Services/DataSeedService.cs`
- **Repository Pattern:** `Repositories/UserRepository.cs`
- **Controller Pattern:** `Controllers/UsersController.cs`
- **Data Management Controller:** `Controllers/DataController.cs`
- **Integration Test Pattern:** `TechHiveAPI.Tests/Integration/Controllers/UsersControllerTests.cs`