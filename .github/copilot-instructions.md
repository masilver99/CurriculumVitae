# GitHub Copilot Instructions

## Project Overview
This is a personal Curriculum Vitae (CV) web application built with ASP.NET Core and Razor Pages. The application displays professional work experience, education, certifications, technical skills, and personal projects. Data is stored in JSON files and loaded into memory at startup for fast retrieval.

## Tech Stack & Key Dependencies
- **Language:** C# 
- **Framework:** ASP.NET Core (NET 10.0)
- **Web Framework:** Razor Pages
- **Logging:** Serilog with file and console sinks
- **Deployment:** Docker (Linux containers)
- **Data Storage:** JSON files (in-memory at runtime)

## Project Structure
- `/CV` - Main application directory
  - `/Pages` - Razor Pages (views and page models)
  - `/Models` - Data models and business logic
  - `/data` - JSON data files and Repository class
  - `/wwwroot` - Static files (CSS, JavaScript, images)
  - `Program.cs` - Application entry point with Serilog configuration
  - `Startup.cs` - Service and middleware configuration
- `CV.sln` - Visual Studio solution file
- `docker-compose.yml` - Docker Compose configuration

## Build & Test Instructions
### Building the Application
```bash
# Build with .NET CLI
dotnet build CV.sln

# Run locally
cd CV
dotnet run

# Build Docker image
docker build -f CV/Dockerfile-release -t cv:latest .

# Run with Docker Compose
docker-compose up
```

### Running Tests
Currently, this project does not have automated tests. When adding tests:
- Use xUnit as the testing framework
- Create a separate test project (e.g., `CV.Tests`)
- Follow AAA (Arrange-Act-Assert) pattern

## Coding Standards & Conventions
### General C# Standards
- Follow standard C# naming conventions:
  - PascalCase for classes, methods, properties, and public members
  - camelCase for private fields (prefixed with `_`)
  - Use descriptive names that clearly indicate purpose
- Add XML documentation comments for public APIs and complex logic
- Use `var` for local variables when the type is obvious
- Prefer LINQ for collection operations
- Use async/await for asynchronous operations

### ASP.NET Core Specific
- Keep Razor Pages focused and thin - move business logic to services or repositories
- Use dependency injection for services (already configured: `Repository`, `VersionInfo`)
- Follow the existing pattern for page models (e.g., `Index.cshtml.cs`)
- Use strongly-typed models for data binding

### Data Access Patterns
- All data is loaded from JSON files in the `data/` directory at startup
- The `Repository` class provides in-memory access to data
- Data models use the pattern: `{Type}Json` for deserialization, `{Type}Item` for runtime models
- Use the existing mapping methods when extending data models
- Cross-references use the `Xref` or `TechXref` properties to link entities

### Error Handling
- Log errors using the configured Serilog logger
- Use appropriate log levels: Debug, Information, Warning, Error, Fatal
- Don't expose sensitive information in error messages or logs
- In development, use detailed exception pages; in production, use generic error pages

## Security Practices
- Never commit secrets, API keys, or connection strings to the repository
- Use User Secrets for local development sensitive data (`UserSecretsId` is configured)
- Validate and sanitize all user input if adding forms
- Keep dependencies up to date for security patches
- Use HTTPS in production (configured in `Startup.cs`)
- The application uses forwarded headers for proper IP address resolution behind proxies

## Docker & Deployment
- The application runs on port 80 (HTTP) and 443 (HTTPS) in containers
- Logs are written to `/app/logs` inside the container, mounted as a volume
- Environment variable `LOG_DIR` controls log directory location
- Use `ASPNETCORE_ENVIRONMENT` to control development vs production behavior
- The Dockerfile uses multi-stage builds for optimized image size

## Documentation Requirements
- Update this instructions file when changing build processes or adding new conventions
- Document complex algorithms or business logic with inline comments
- Keep README.md updated if adding setup or usage instructions
- Use XML comments for public APIs

## Data Management
### JSON Data Files
All data files are located in `CV/data/`:
- `work.json` - Work experience entries
- `ed.json` - Education entries
- `projects.json` - Personal projects
- `tech.json` - Technical skills organized by category
- `certifications.json` - Professional certifications

When modifying JSON files:
- Maintain the existing schema structure
- Use `JsonCommentHandling.Skip` and `AllowTrailingCommas` options
- IDs should be unique and used for cross-referencing
- Dates should be parseable by `DateTime.TryParse`

### Adding New Data Types
If adding new data types:
1. Create JSON DTO class (e.g., `{Type}Json`)
2. Create runtime model class (e.g., `{Type}Item`)
3. Add collection to `Repository` constructor
4. Implement Get and Map methods following existing patterns
5. Add corresponding Razor Pages if needed

## Prohibited Practices
- Don't remove or modify the Serilog configuration without good reason
- Don't add unnecessary NuGet packages without justification
- Avoid blocking I/O operations in request handling paths
- Don't store sensitive data in JSON files
- Don't bypass the Repository pattern for data access
- Avoid hardcoding URLs, paths, or configuration values

## Common Tasks
### Adding a New Page
1. Create `.cshtml` and `.cshtml.cs` files in `/CV/Pages`
2. Inherit from `PageModel`
3. Inject required services via constructor
4. Follow existing page patterns for consistency

### Adding a New JSON Data Type
1. Define JSON DTO and runtime model in `/CV/Models`
2. Add data file to `/CV/data`
3. Update `Repository` class with load and mapping logic
4. Create corresponding Razor Page for display

### Updating Dependencies
```bash
# Check for updates
dotnet list package --outdated

# Update a specific package
dotnet add package <PackageName> --version <Version>
```

## Performance Considerations
- All data is loaded into memory at startup for fast access
- Use async methods for I/O operations
- Leverage LINQ for efficient data filtering and transformation
- Static files are served directly from `wwwroot`
- Consider caching for computationally expensive operations

## Logging Best Practices
- Use structured logging with Serilog
- Include relevant context in log messages
- Use appropriate log levels consistently
- Logs are written to both console and rotating files
- Check `LOG_DIR` environment variable for custom log location
