// Program.cs - נקודת הכניסה הראשית של האפליקציה ASP.NET Core
// קובץ זה מגדיר את כל ההגדרות של האפליקציה כולל:
// - הגדרות לוגינג עם Serilog
// - חיבור ל-MongoDB ול-SQL Server
// - הגדרות אימות JWT
// - חיבור ל-Redis לצורך קאשינג
// - רישום כל השירותים והרפוזיטוריז ב-DI Container
// - הגדרות CORS לאפליקציית Angular
// - הגדרות Swagger לתיעוד API

using System.Text;
using ChineseAuctionAPI.Data;
using ChineseAuctionAPI.Repositories;
using ChineseAuctionAPI.Services;
using ChineseAuctionAPI.Services.Caching;
using ChineseAuctionAPI.Services.RateLimiting;
using ChineseAuctionAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Serilog;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1. ����� ����� - ��� ����� �-LogEventLevel ��� ����� ����� ������
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // ����� ������ ������� ������� (Strings) - �-Visual Studio �� ���� �� ����� ������
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/all_requests.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting web host - Monitoring all requests");

    builder.Host.UseSerilog();

    // ===== Register Services (JWT, Auth, Scoped) =====
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var keyString = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
    var key = Encoding.UTF8.GetBytes(keyString);

    // משיכת ההגדרות מהקובץ שהגדרנו קודם
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings");

    // רישום הלקוח (Client) כ-Singleton (נוצר פעם אחת לכל אורך חיי האפליקציה)
    builder.Services.AddSingleton<IMongoClient>(sp =>
        new MongoClient(mongoSettings.GetValue<string>("ConnectionString")));

    // רישום בסיס הנתונים הספציפי
    builder.Services.AddScoped(sp => {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(mongoSettings.GetValue<string>("DatabaseName"));
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    });

    // ===== Register Rate Limiting (Sliding Window) =====
    builder.Services.AddRateLimiter(rateLimiterOptions =>
    {
        rateLimiterOptions.AddPolicy("SlidingWindowLimiter", httpContext =>
            System.Threading.RateLimiting.RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                factory: partition => new System.Threading.RateLimiting.SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 8,
                    QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                }));

        rateLimiterOptions.AddPolicy("SlidingWindowLimiterPerIP", httpContext =>
            System.Threading.RateLimiting.RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                factory: partition => new System.Threading.RateLimiting.SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 200,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 8,
                    QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                }));

        rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // ===== Register Redis Cache =====
    var redisConfiguration = builder.Configuration.GetSection("Redis");
    var redisHost = redisConfiguration["Host"] ?? "localhost";
    var redisPort = int.Parse(redisConfiguration["Port"] ?? "6379");
    var redisPassword = redisConfiguration["Password"] ?? "YourSecurePassword123!";

    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { $"{redisHost}:{redisPort}" },
        Password = redisPassword,
        AllowAdmin = true,
        Ssl = false,
        AbortOnConnectFail = false
    };

    var redisConnection = ConnectionMultiplexer.Connect(configurationOptions);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
    builder.Services.AddScoped<ICacheService, RedisCacheService>();

    // ===== Register Rate Limiting Service (Sliding Window) =====
    builder.Services.AddScoped<IRateLimitingService, RateLimitingService>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChineseAuctionAPI", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        });
    });

    // Register Repositories & Services
    builder.Services.AddScoped<IUserRepo, UserRepo>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IOrderRepo, MongoOrderRepo>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IGiftCategoryRepo, GiftCategoryRepo>();
    builder.Services.AddScoped<IGiftCategoryService, GiftCategoryService>();
    builder.Services.AddScoped<IGiftRepo, GiftRepo>();
    builder.Services.AddScoped<IGiftService, GiftService>();
    builder.Services.AddScoped<IDonorRepository, DonorRepository>();
    builder.Services.AddScoped<IDonorService, DonorService>();
    builder.Services.AddScoped<IPackageRepo, PackageRepo>();
    builder.Services.AddScoped<IPackageService, PackageService>();
    builder.Services.AddScoped<IEmailService1, EmailService>();
    builder.Services.AddScoped<MongoMigrationService>();
    builder.Services.AddScoped<MongoOrderQueryService>();


    var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
    builder.Services.AddDbContext<SaleContextDB>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular",
            policy => policy.AllowAnyOrigin() // �� ������ �-http://localhost:4200
                            .AllowAnyMethod()
                            .AllowAnyHeader());
    });
    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var payload = System.Text.Json.JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            await context.Response.WriteAsync(payload);
        });
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChineseAuctionAPI v1"));
    }
    app.UseCors("AllowAngular");
    app.UseHttpsRedirection();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "public")),
        RequestPath = "" 
    });

    // Rate Limiting Middleware
    app.UseRateLimiter();

    // Middleware: Extract JWT from HttpOnly Cookie and add as Bearer Token
    app.Use(async (context, next) =>
    {
        if (context.Request.Cookies.TryGetValue("authToken", out var token) && string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            Log.Information("JWT token extracted from authToken cookie and added to Authorization header");
        }
        await next();
    });

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}