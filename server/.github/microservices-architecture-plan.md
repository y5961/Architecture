# Microservices Architecture Plan - Chinese Auction System

## Overview
This document outlines a strategic plan to evolve the Chinese Auction monolithic application into a microservices architecture. The transition aims to improve scalability, maintainability, and fault isolation while reducing coupling between business domains.

## Current Architecture (Monolith)
Currently, the Chinese Auction system is a monolithic ASP.NET Core application combining:
- User & Authentication management
- Gift catalog management
- Gift category management
- Donor management
- Order processing & management
- Package management
- Email notifications

All services share a single database context and are deployed as one unit.

## Proposed Microservices Architecture

### Phase 1: Logical Service Extraction (Current State)
Before full deployment separation, organize code into service-oriented layers:

```
ChineseAuctionAPI (Monolithic Backend)
├── Controllers/
│   ├── UserController
│   ├── AuthController (new)
│   ├── GiftController
│   ├── GiftCategoryController
│   ├── DonorController
│   ├── OrderController
│   └── PackageController
├── Services/
│   ├── AuthenticationService (JWT, tokens)
│   ├── UserService
│   ├── GiftService
│   ├── GiftCategoryService
│   ├── DonorService
│   ├── OrderService
│   ├── PackageService
│   ├── NotificationService (Email)
│   └── Caching/ (Redis cache management)
├── Repositories/
│   └── MongoDB & SQL Server data access
└── Data/
    ├── SaleContextDB (SQL Server EF Core)
    └── MongoDB migration & query services
```

### Phase 2: Microservices Breakdown (Future)

```
┌─────────────────────────────────────────────────────────────┐
│                     API Gateway                             │
│  • Request routing                                          │
│  • Rate limiting & throttling                               │
│  • Authentication validation                                │
│  • CORS handling                                            │
└─────────────────────────────────────────────────────────────┘
                             │
        ┌────────┬───────────┼────────┬──────────────┐
        ↓        ↓           ↓        ↓              ↓
    ┌────────┐ ┌─────────┐ ┌──────┐ ┌──────────┐ ┌──────────┐
    │ Auth   │ │ Product │ │Order │ │ Donor    │ │Notif     │
    │Service │ │ Service │ │Svc   │ │Service   │ │Service   │
    └────────┘ └─────────┘ └──────┘ └──────────┘ └──────────┘
        │           │          │          │          │
        ↓           ↓          ↓          ↓          ↓
    [AuthDB]  [ProductDB] [OrderDB] [DonorDB]  [EmailQueue]
```

### Proposed Microservices

#### 1. **Authentication Service**
- **Responsibilities**:
  - User login/registration
  - JWT token generation and validation
  - Role-based access control (RBAC)
  - Session management
  - Password hashing & verification
  
- **API Endpoints**:
  - `POST /auth/register`
  - `POST /auth/login`
  - `POST /auth/logout`
  - `POST /auth/refresh-token`
  - `GET /auth/validate-token`
  
- **Data**: User credentials, roles, tokens
- **Database**: SQL Server (Users table)
- **Security**: HttpOnly cookies, JWT Bearer tokens, BCrypt

#### 2. **Product Service**
- **Responsibilities**:
  - Gift management (CRUD)
  - Gift category management
  - Gift availability & inventory
  - Package management
  
- **API Endpoints**:
  - `GET /products/gifts`
  - `GET /products/gifts/{id}`
  - `POST /products/gifts`
  - `PUT /products/gifts/{id}`
  - `DELETE /products/gifts/{id}`
  - `GET /products/categories`
  - `GET /products/packages`
  
- **Data**: Gifts, categories, packages
- **Database**: SQL Server + MongoDB (for complex gift relationships)
- **Caching**: Redis (gift:id, gift:all with TTL)

#### 3. **Order Service**
- **Responsibilities**:
  - Order creation & tracking
  - Order status management
  - Order history retrieval
  - Payment processing integration
  
- **API Endpoints**:
  - `POST /orders`
  - `GET /orders/{id}`
  - `GET /orders/user/{userId}`
  - `PUT /orders/{id}/status`
  - `GET /orders/{id}/history`
  
- **Data**: Orders, order items, order status
- **Database**: MongoDB (orders collection) + SQL Server (transactions)
- **Events**: OrderCreated, OrderShipped, OrderCompleted (for async communication)

#### 4. **Donor Service**
- **Responsibilities**:
  - Donor profile management
  - Donor gift associations
  - Donor statistics & reporting
  - Thank-you notifications
  
- **API Endpoints**:
  - `GET /donors`
  - `GET /donors/{id}`
  - `POST /donors`
  - `PUT /donors/{id}`
  - `DELETE /donors/{id}`
  - `GET /donors/{id}/gifts`
  
- **Data**: Donor information, donation history
- **Database**: SQL Server
- **Caching**: Redis (donor:id, donor:all with TTL)

#### 5. **Notification Service**
- **Responsibilities**:
  - Email notifications
  - SMS alerts (future)
  - Notification templates
  - Delivery tracking
  
- **API Endpoints** (Internal):
  - `POST /notifications/email`
  - `POST /notifications/sms`
  - `GET /notifications/status/{id}`
  
- **Data**: Notification templates, delivery logs
- **Database**: SQL Server (audit log)
- **Integration**: Email provider (SMTP), message queue (RabbitMQ/Azure Service Bus)

### Communication Patterns

#### Synchronous Communication (HTTP/REST)
- Client → API Gateway → Service
- Service → Service (within timeout constraints)
- Example: Authentication Service validates token for Product Service

#### Asynchronous Communication (Event-Driven)
- Service publishes events to message broker
- Other services subscribe and react
- Examples:
  - Order Service publishes "OrderCreated" → Notification Service sends confirmation
  - Product Service publishes "GiftUpdated" → Cache invalidation
  
- **Tools**: RabbitMQ, Azure Service Bus, or Kafka

#### Service Discovery
- Use service registry (Consul, Eureka) or Kubernetes DNS
- API Gateway routes requests to appropriate service instances

### Data Management Strategy

#### Database per Service Principle
Each service owns its database to ensure loose coupling:
- Auth Service → SQL Server (Users)
- Product Service → SQL Server (Gifts, Categories, Packages)
- Order Service → MongoDB (Orders) + SQL Server (Order metadata)
- Donor Service → SQL Server (Donors)

#### Data Consistency
- **Eventual Consistency**: Event-driven updates across services
- **Saga Pattern**: Multi-step transactions coordinated via events
  - Example: Order saga → Reserve inventory → Process payment → Ship package

#### Shared Data (Transitional)
During migration, shared access patterns:
- Cache (Redis) for frequently accessed data
- Events for eventual consistency
- Gradual data duplication to specific services

### Deployment Strategy

#### Phase 2a: Containerized Monolith (Current)
```
docker-compose.yml
├── ChineseAuctionAPI (single service)
├── MongoDB
├── SQL Server
└── Redis
```

#### Phase 2b: Containers + Service Separation (Year 2)
```
docker-compose.yml
├── API Gateway
├── AuthService
├── ProductService
├── OrderService
├── DonorService
├── NotificationService
├── MongoDB
├── SQL Server
├── Redis
└── RabbitMQ (message broker)
```

#### Phase 2c: Kubernetes Orchestration (Year 3+)
- Deploy services as Kubernetes pods
- Service mesh (Istio) for inter-service communication
- Horizontal auto-scaling per service
- Zero-downtime deployments

### API Gateway Responsibilities

```csharp
// Pseudo-code for API Gateway configuration
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Authentication routes
    endpoints.MapAuthService("http://auth-service:5002");
    
    // Product routes
    endpoints.MapProductService("http://product-service:5003");
    
    // Order routes
    endpoints.MapOrderService("http://order-service:5004");
    
    // Donor routes
    endpoints.MapDonorService("http://donor-service:5005");
});
```

### Monitoring & Logging Strategy

#### Centralized Logging
- All services write to centralized logging (ELK Stack, Application Insights)
- Correlation IDs for request tracing across services
- Serilog with structured logging

#### Health Checks
- Each service implements `/health` endpoint
- API Gateway monitors service health
- Automatic failover to healthy instances

#### Metrics & Observability
- Prometheus for metrics collection
- Grafana for visualization
- Service-level SLAs & performance tracking

### Migration Path

**Year 1 (Current)**: Organize monolith with service layers
- Implement repositories properly
- Add service interfaces
- Improve logging & monitoring

**Year 2**: Logical service extraction
- Extract to separate projects (still monolithic deployment)
- Implement event-driven communication internally
- Database schema normalization

**Year 3**: Containerized services
- Separate Docker containers per service
- Implement API Gateway (Ocelot or Kong)
- Event broker (RabbitMQ)

**Year 4+**: Kubernetes & scaling
- Deploy to K8s cluster
- Service mesh implementation
- Advanced monitoring & auto-scaling

### Cost-Benefit Analysis

#### Benefits
- **Scalability**: Scale individual services based on demand
- **Resilience**: Failure isolation (one service down ≠ entire system down)
- **Agility**: Independent deployment & technology choices
- **Team Autonomy**: Small teams own services end-to-end
- **Performance**: Optimize each service independently

#### Challenges
- **Complexity**: Distributed system debugging & monitoring
- **Network Latency**: Inter-service communication overhead
- **Data Consistency**: Eventual consistency challenges
- **Operational Overhead**: Multiple services to manage
- **Testing**: Integration & end-to-end testing complexity

### Recommendations

1. **Start with Phase 1**: Organize current code properly before splitting services
2. **Invest in logging/monitoring**: Critical for microservices success
3. **Build API Gateway gradually**: Start with simple routing, add features incrementally
4. **Consider Domain-Driven Design (DDD)**: Define clear service boundaries
5. **Use API versioning**: Prepare for service API changes
6. **Implement circuit breakers**: Prevent cascading failures
7. **Plan for data migration**: Consider how data flows between services
8. **Document service contracts**: OpenAPI specs for each service

---

## Conclusion
The proposed microservices architecture provides a scalable path for the Chinese Auction system's growth. However, this transition should be gradual, beginning with proper logical organization within the current monolith before moving to physical separation. Each phase should include comprehensive testing and monitoring improvements.
