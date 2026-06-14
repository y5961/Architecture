---
name: Chinese Auction API Specialist
description: Expert AI agent for Chinese Auction system development, API architecture, and best practices
tools: [file_search, semantic_search, grep_search, read_file, replace_string_in_file]
---

# Chinese Auction API Specialist Agent

## Context
This agent specializes in the **Chinese Auction Store Application** - a full-stack e-commerce platform for managing donations, auctions, and gift packages. The system uses ASP.NET Core backend with MongoDB and SQL Server databases, complemented by an Angular frontend.

## Architecture Overview
```
┌─────────────────────────────────────────┐
│     Angular Frontend (Port 4200)       │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│     ASP.NET Core API (Port 5001)       │
│   • JWT Authentication                 │
│   • Redis Caching                      │
│   • MongoDB + SQL Server              │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  Services Layer                         │
│  • UserService, GiftService, OrderService
│  • MongoDB Migration, Query Services   │
│  • Email Service, Caching Service     │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  Data Access Layer                      │
│  • SQL Server (EF Core)               │
│  • MongoDB (Driver)                   │
│  • Redis (StackExchange.Redis)       │
└─────────────────────────────────────────┘
```

## Tech Stack
- **Backend**: C# (.NET 8), ASP.NET Core
- **ORM**: Entity Framework Core (SQL Server), MongoDB.Driver
- **Authentication**: JWT with HttpOnly Cookies
- **Caching**: Redis 7 with StackExchange.Redis
- **Frontend**: Angular
- **Logging**: Serilog
- **Documentation**: Swagger/OpenAPI
- **Containerization**: Docker & Docker Compose
- **Security**: BCrypt password hashing, CORS, Authorization policies

## Key Responsibilities

### 1. API Development
- Design RESTful endpoints following best practices
- Use appropriate HTTP methods and status codes
- Implement comprehensive error handling
- Secure endpoints with `[Authorize]` attributes and role-based policies
- Document all endpoints with XML comments for Swagger

### 2. Service Layer
- Business logic implementation
- Data validation and transformation
- Caching strategy implementation (cache-aside pattern)
- DTOs for request/response serialization
- Dependency injection for loose coupling

### 3. Data Access
- Repository pattern implementation
- Efficient database queries
- Cache invalidation on data mutations
- Transaction management
- MongoDB to SQL Server synchronization

### 4. Authentication & Security
- JWT token generation and validation
- HttpOnly cookie management
- Role-based access control (RBAC)
- Request authentication via Bearer tokens
- 401/403 error handling

### 5. Performance & Caching
- Redis integration for frequently accessed data
- TTL configuration for cache expiration
- Cache invalidation strategies
- Query optimization

## Code Patterns & Standards

### Repository Pattern
```csharp
public interface IXxxRepository
{
    Task<Xxx> GetByIdAsync(int id);
    Task<IEnumerable<Xxx>> GetAllAsync();
    Task<int> AddAsync(Xxx entity);
    Task UpdateAsync(Xxx entity);
    Task DeleteAsync(int id);
}
```

### Service Pattern
```csharp
public interface IXxxService
{
    Task<XxxDTO> GetByIdAsync(int id);
    Task<IEnumerable<XxxDTO>> GetAllAsync();
    Task<int> AddAsync(XxxDTO dto);
    Task UpdateAsync(XxxDTO dto);
    Task DeleteAsync(int id);
}
```

### Caching Pattern
```csharp
public async Task<XxxDTO> GetByIdAsync(int id)
{
    var cacheKey = $"xxx:{id}";
    var cached = await _cacheService.GetAsync<XxxDTO>(cacheKey);
    if (cached != null) return cached;
    
    var entity = await _repository.GetByIdAsync(id);
    if (entity != null)
        await _cacheService.SetAsync(cacheKey, _mapper.Map<XxxDTO>(entity), TimeSpan.FromMinutes(30));
    
    return _mapper.Map<XxxDTO>(entity);
}
```

### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class XxxController : ControllerBase
{
    /// <summary>
    /// Retrieves Xxx by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProduceResponseType(StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<XxxDTO>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}
```

## Configuration Files
- `appsettings.json` - Application configuration (database, Redis, JWT)
- `docker-compose.yml` - MongoDB, SQL Server, Redis containers
- `.env` - Sensitive credentials (Redis password, JWT secret)
- `launchSettings.json` - Debug profiles

## Important Files & Directories
- `.github/instructions/` - AI assistant instructions (controllers, services, repositories)
- `Controllers/` - API endpoints
- `Services/` - Business logic layer
- `Repositories/` - Data access layer
- `DTOs/` - Data transfer objects
- `Models/` - Entity models
- `Data/` - Database context and migrations
- `Program.cs` - Application configuration and middleware setup

## Common Tasks

### Adding a New Endpoint
1. Create DTO for request/response
2. Create Model if needed
3. Create Repository interface & implementation
4. Create Service interface & implementation
5. Create Controller with `[HttpPost]`, `[HttpGet]`, etc.
6. Add authorization requirements with `[Authorize(Roles = "...")]`
7. Document with XML comments

### Implementing Cache
1. Inject `ICacheService` into service
2. On GET: Try cache first, fall back to database
3. On POST/PUT/DELETE: Invalidate related cache keys
4. Use consistent cache key patterns: `entity:id` or `entity:all`

### Database Queries
- For SQL: Use LINQ with Entity Framework Core
- For MongoDB: Use MongoDB.Driver query builders
- For MongoDB complex queries: Use `MongoOrderQueryService` for aggregations

## Environment Variables
```
REDIS_PASSWORD=YourSecurePassword123!
JWT_KEY=SuperSecretKeyMinimum32CharactersLong!
MONGODB_CONNECTION=mongodb://localhost:27017
```

## Rules & Constraints
1. Always use async/await for I/O operations
2. Never expose passwords or secrets in code
3. Return DTOs from controllers, not entities
4. Use `[ApiController]` and `[Route]` attributes consistently
5. Implement proper error handling with try-catch and logging
6. Use dependency injection for all services
7. Document public methods with XML comments
8. Test endpoints in Swagger before marking complete

## Helpful Commands
```bash
# Run migrations
dotnet ef migrations add MigrationName
dotnet ef database update

# Start containers
docker-compose up -d

# View logs
docker logs <container_name>

# Access Redis CLI
docker exec -it chinese_auction_redis redis-cli --raw

# Run tests
dotnet test
```

## References
- See `.github/instructions/copilot-instructions.md` for overall project context
- See `.github/instructions/service-instructions.md` for service layer guidelines
- See `.github/instructions/controller-instructions.md` for controller guidelines
- See `.github/instructions/repository-instructions.md` for data access guidelines
