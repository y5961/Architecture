ChineseAuction.Api.Angular/
├── server/
│   ├── ChineseAuctionAPI/                    # ❌ MONOLITH (Legacy - Weeks 0-52)
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── Repositories/
│   │   └── Program.cs
│   │
│   └── Microservices/                         # ✅ NEW SERVICES START HERE
│       │
│       ├── 1_AuthService/                     # Phase 2 (Weeks 4-8)
│       │   ├── src/
│       │   │   ├── ChineseAuction.AuthService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── AuthController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── AuthService.cs
│       │   │   │   │   └── JwtTokenService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── User.cs
│       │   │   │   │   └── RegisterRequest.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── AuthContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.AuthService.csproj
│       │   ├── tests/
│       │   │   └── AuthService.Tests/
│       │   ├── Dockerfile
│       │   ├── docker-compose.yml
│       │   └── README.md
│       │
│       ├── 2_DonorService/                    # Phase 3 (Weeks 9-14)
│       │   ├── src/
│       │   │   ├── ChineseAuction.DonorService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── DonorController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   └── DonorService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   └── Donor.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── DonorContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   ├── DonorCreatedEvent.cs
│       │   │   │   │   ├── DonorUpdatedEvent.cs
│       │   │   │   │   └── DonorDeletedEvent.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.DonorService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 3_PackageService/                  # Phase 3 (Weeks 9-14)
│       │   ├── src/
│       │   │   ├── ChineseAuction.PackageService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── PackageController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   └── PackageService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   └── Package.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── PackageContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   ├── PackageCreatedEvent.cs
│       │   │   │   │   ├── PackageUpdatedEvent.cs
│       │   │   │   │   └── PackageDeletedEvent.cs
│       │   │   │   ├── Cache/
│       │   │   │   │   └── RedisPackageCache.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.PackageService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 4_GiftService/                     # Phase 4 (Weeks 15-21)
│       │   ├── src/
│       │   │   ├── ChineseAuction.GiftService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   ├── GiftController.cs
│       │   │   │   │   └── GiftCategoryController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── GiftService.cs
│       │   │   │   │   ├── GiftCategoryService.cs
│       │   │   │   │   ├── S3FileUploadService.cs
│       │   │   │   │   └── DonorServiceClient.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── Gift.cs
│       │   │   │   │   └── GiftCategory.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── GiftContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   ├── GiftCreatedEvent.cs
│       │   │   │   │   ├── GiftUpdatedEvent.cs
│       │   │   │   │   ├── GiftDeletedEvent.cs
│       │   │   │   │   └── DonorDeletedEventHandler.cs
│       │   │   │   ├── Resilience/
│       │   │   │   │   └── DonorServiceCircuitBreaker.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.GiftService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 5_OrderService/                    # Phase 5 (Weeks 22-35) ⭐ CRITICAL
│       │   ├── src/
│       │   │   ├── ChineseAuction.OrderService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── OrderController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── OrderService.cs
│       │   │   │   │   ├── OrderDraftService.cs
│       │   │   │   │   ├── GiftServiceClient.cs
│       │   │   │   │   ├── PackageServiceClient.cs
│       │   │   │   │   └── DraftLockService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── Order.cs
│       │   │   │   │   ├── OrdersGift.cs
│       │   │   │   │   └── OrdersPackage.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── OrderContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   ├── OrderDraftCreatedEvent.cs
│       │   │   │   │   ├── OrderCompletedEvent.cs
│       │   │   │   │   ├── OrderCancelledEvent.cs
│       │   │   │   │   ├── EventPublisher.cs
│       │   │   │   │   └── Handlers/
│       │   │   │   │       ├── GiftDeletedEventHandler.cs
│       │   │   │   │       └── PackageUpdatedEventHandler.cs
│       │   │   │   ├── Saga/
│       │   │   │   │   ├── CompleteOrderSaga.cs
│       │   │   │   │   ├── OrderSagaState.cs
│       │   │   │   │   └── SagaOrchestrator.cs
│       │   │   │   ├── Resilience/
│       │   │   │   │   └── ServiceClientCircuitBreaker.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.OrderService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 6_ReportService/                   # Phase 5 (Weeks 22-35) CQRS Read Model
│       │   ├── src/
│       │   │   ├── ChineseAuction.ReportService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── ReportController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── IncomeReportService.cs
│       │   │   │   │   ├── ReportSummaryService.cs
│       │   │   │   │   └── EventAggregationService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── IncomeReport.cs
│       │   │   │   │   ├── DailySummary.cs
│       │   │   │   │   └── CategorySummary.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── ReportContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   └── Handlers/
│       │   │   │   │       ├── OrderCompletedEventHandler.cs
│       │   │   │   │       └── OrderCancelledEventHandler.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.ReportService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 7_WinnerService/                   # Phase 6 (Weeks 36-44)
│       │   ├── src/
│       │   │   ├── ChineseAuction.WinnerService/
│       │   │   │   ├── Controllers/
│       │   │   │   │   └── WinnerController.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── WinnerService.cs
│       │   │   │   │   ├── RaffleService.cs
│       │   │   │   │   └── GiftServiceClient.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── Winner.cs
│       │   │   │   │   └── WinnerAuditLog.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── WinnerContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   ├── UserWonEvent.cs
│       │   │   │   │   └── Handlers/
│       │   │   │   │       └── OrderCompletedEventHandler.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.WinnerService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       ├── 8_NotificationService/             # Phase 6 (Weeks 36-44)
│       │   ├── src/
│       │   │   ├── ChineseAuction.NotificationService/
│       │   │   │   ├── Services/
│       │   │   │   │   ├── EmailService.cs
│       │   │   │   │   ├── SESEmailProvider.cs
│       │   │   │   │   ├── TemplateService.cs
│       │   │   │   │   └── RetryService.cs
│       │   │   │   ├── Models/
│       │   │   │   │   ├── EmailTemplate.cs
│       │   │   │   │   ├── NotificationLog.cs
│       │   │   │   │   └── RetryLog.cs
│       │   │   │   ├── Data/
│       │   │   │   │   ├── NotificationContext.cs
│       │   │   │   │   └── Migrations/
│       │   │   │   ├── Events/
│       │   │   │   │   └── Handlers/
│       │   │   │   │       ├── UserWonEventHandler.cs
│       │   │   │   │       ├── OrderCompletedEventHandler.cs
│       │   │   │   │       └── UserRegisteredEventHandler.cs
│       │   │   │   ├── SqsConsumer/
│       │   │   │   │   └── NotificationQueueListener.cs
│       │   │   │   ├── Program.cs
│       │   │   │   └── appsettings.json
│       │   │   └── ChineseAuction.NotificationService.csproj
│       │   ├── tests/
│       │   ├── Dockerfile
│       │   └── README.md
│       │
│       └── Shared/                            # ✅ SHARED LIBRARIES
│           ├── ChineseAuction.Shared/
│           │   ├── Events/
│           │   │   ├── IEvent.cs
│           │   │   ├── BaseEvent.cs
│           │   │   └── EventBus/
│           │   │       ├── IEventBus.cs
│           │   │       ├── AwsSnsEventBus.cs
│           │   │       └── EventPublisher.cs
│           │   ├── DTOs/
│           │   │   ├── UserDTO.cs
│           │   │   ├── DonorDTO.cs
│           │   │   ├── GiftDTO.cs
│           │   │   ├── PackageDTO.cs
│           │   │   └── OrderDTO.cs
│           │   ├── Exceptions/
│           │   │   ├── ServiceException.cs
│           │   │   ├── NotFoundException.cs
│           │   │   └── ValidationException.cs
│           │   ├── Middleware/
│           │   │   ├── ErrorHandlingMiddleware.cs
│           │   │   ├── LoggingMiddleware.cs
│           │   │   └── JwtValidationMiddleware.cs
│           │   ├── Configuration/
│           │   │   ├── ServiceCollectionExtensions.cs
│           │   │   └── ServiceConfiguration.cs
│           │   ├── HttpClients/
│           │   │   ├── AuthServiceHttpClient.cs
│           │   │   ├── DonorServiceHttpClient.cs
│           │   │   ├── GiftServiceHttpClient.cs
│           │   │   └── PackageServiceHttpClient.cs
│           │   └── ChineseAuction.Shared.csproj
│       │
│       └── Infrastructure/                    # ✅ INFRA & DEVOPS
│           ├── docker-compose.yml             # Local dev environment
│           ├── kubernetes/
│           │   ├── api-gateway.yaml
│           │   ├── auth-service.yaml
│           │   ├── donor-service.yaml
│           │   ├── package-service.yaml
│           │   ├── gift-service.yaml
│           │   ├── order-service.yaml
│           │   ├── report-service.yaml
│           │   ├── winner-service.yaml
│           │   ├── notification-service.yaml
│           │   └── ingress.yaml
│           ├── terraform/
│           │   ├── main.tf
│           │   ├── vpc.tf
│           │   ├── rds.tf
│           │   ├── sqs_sns.tf
│           │   ├── s3.tf
│           │   └── ecs.tf
│           ├── github-actions/
│           │   ├── build-and-test.yml
│           │   ├── deploy-auth-service.yml
│           │   ├── deploy-donor-service.yml
│           │   ├── deploy-gift-service.yml
│           │   ├── deploy-order-service.yml
│           │   ├── deploy-report-service.yml
│           │   ├── deploy-winner-service.yml
│           │   └── deploy-notification-service.yml
│           └── scripts/
│               ├── setup-aws.sh
│               ├── migrate-databases.sh
│               └── health-check.sh
│
├── .github/
│   ├── plamMultyServices.md                   # This plan
│   ├── instructions/
│   │   ├── controller-instructions.md
│   │   ├── service-instructions.md
│   │   ├── microservice-standards.md
│   │   └── event-driven-patterns.md
│   └── workflows/
│
└── documentation/
    ├── ARCHITECTURE.md
    ├── API_CONTRACTS.md
    ├── DATABASE_SCHEMA.md
    ├── EVENT_CATALOG.md
    └── DEPLOYMENT_GUIDE.md