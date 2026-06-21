using Customer_Managerment.CustomerManagement.Api.Hubs;
using Customer_Managerment.CustomerManagement.Api.Services;
using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Api.MiddleWare;
using Customer_Managerment.CustomerManagement.Api.Mutation;
using Customer_Managerment.CustomerManagement.Api.Query;
using Customer_Managerment.CustomerManagement.Api.Types;
using Customer_Managerment.CustomerManagement.Application.Handlers.Auth;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Application.UseCases.Authen;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Repositories;
using Customer_Managerment.CustomerManagement.Infrastructure.Services;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

DotNetEnv.Env.Load();

void SetConfigIfEnvNotEmpty(string configKey, string envVarName)
{
    var value = Environment.GetEnvironmentVariable(envVarName);
    if (!string.IsNullOrEmpty(value))
    {
        builder.Configuration[configKey] = value;
    }
}

SetConfigIfEnvNotEmpty("ConnectionStrings:PostgreSQLConnection", "CONNECTIONSTRINGS__PostgreSQLConnection");
SetConfigIfEnvNotEmpty("JwtSettings:SecretKey", "APPSETTINGS__SECRETKEY");
SetConfigIfEnvNotEmpty("JwtSettings:Issuer", "APPSETTINGS__ISSUER");
SetConfigIfEnvNotEmpty("JwtSettings:Audience", "APPSETTINGS__AUDIENCE");
SetConfigIfEnvNotEmpty("JwtSettings:AccessTokenExpirationMinutes", "APPSETTINGS__ACCESSTOKENEXP");
SetConfigIfEnvNotEmpty("JwtSettings:RefreshTokenExpirationDays", "APPSETTINGS__REFRESHTOKENEXP");
SetConfigIfEnvNotEmpty("RedisSettings:Host", "REDISSETTINGS__HOST");
SetConfigIfEnvNotEmpty("RedisSettings:Port", "REDISSETTINGS__PORT");
SetConfigIfEnvNotEmpty("RedisSettings:Password", "REDISSETTINGS__PASSWORD");
SetConfigIfEnvNotEmpty("SendGrid:ApiKey", "SENDER_APIKEY");
SetConfigIfEnvNotEmpty("SendGrid:Email", "SENDER_EMAIL");
SetConfigIfEnvNotEmpty("SendGrid:Name", "SENDER_NAME");
SetConfigIfEnvNotEmpty("Elasticsearch:Uri", "ES__URL");
SetConfigIfEnvNotEmpty("GroqSettings:ApiKey", "GROQ__APIKEY");
SetConfigIfEnvNotEmpty("GroqSettings:BaseUrl", "GROQ__APIURL");

// Fail-fast on missing/invalid production secrets. Catches empty placeholders from appsettings.json
// when env vars are misconfigured. Local dev with secrets supplied via .env or user-secrets passes through.
ValidateCriticalConfiguration(builder.Configuration);

static void ValidateCriticalConfiguration(IConfiguration config)
{
    var errors = new List<string>();

    var pgConn = config.GetConnectionString("PostgreSQLConnection");
    if (string.IsNullOrWhiteSpace(pgConn))
    {
        errors.Add("ConnectionStrings:PostgreSQLConnection is empty.");
    }
    else
    {
        var placeholders = new[] { "REPLACE_ME", "YOUR_PASSWORD", "CHANGEME", "PLACEHOLDER", "<password>", "{password}" };
        var lower = pgConn.ToLowerInvariant();
        if (placeholders.Any(p => lower.Contains(p.ToLowerInvariant())))
            errors.Add("ConnectionStrings:PostgreSQLConnection contains placeholder text (e.g. REPLACE_ME, CHANGEME).");
    }

    var jwtSecret = config["JwtSettings:SecretKey"];
    if (string.IsNullOrWhiteSpace(jwtSecret))
        errors.Add("JwtSettings:SecretKey is empty.");
    else if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
        errors.Add($"JwtSettings:SecretKey is too short ({Encoding.UTF8.GetByteCount(jwtSecret)} bytes). HS256 requires >= 32 bytes. Generate with: openssl rand -hex 32");

    var issuer = config["JwtSettings:Issuer"];
    if (string.IsNullOrWhiteSpace(issuer)) errors.Add("JwtSettings:Issuer is empty.");

    var audience = config["JwtSettings:Audience"];
    if (string.IsNullOrWhiteSpace(audience)) errors.Add("JwtSettings:Audience is empty.");

    var groqKey = config["GroqSettings:ApiKey"];
    if (string.IsNullOrWhiteSpace(groqKey)) errors.Add("GroqSettings:ApiKey is empty (chatbot will fail at runtime).");

    if (errors.Count > 0)
    {
        var msg = "CRITICAL configuration errors detected. Refusing to start:\n  - " + string.Join("\n  - ", errors);
        throw new InvalidOperationException(msg);
    }
}


// DbContext Registration
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPooledDbContextFactory<CustomerManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"))
);


// Health Checks (DB + Redis). Returns 200 OK with per-check status; used by Docker HEALTHCHECK and uptime monitors.
var pgConn = builder.Configuration.GetConnectionString("PostgreSQLConnection");
var redisHost = builder.Configuration["RedisSettings:Host"];
var redisPort = builder.Configuration["RedisSettings:Port"];
var redisPassword = builder.Configuration["RedisSettings:Password"];
var redisConn = $"{redisHost}:{redisPort},password={redisPassword},ssl=true,abortConnect=false";

builder.Services.AddHealthChecks()
    .AddNpgSql(pgConn!, name: "postgres", tags: new[] { "db", "ready" })
    .AddRedis(redisConn, name: "redis", tags: new[] { "cache", "ready" });


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDealRepository, DealRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteMentionRepository, NoteMentionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IStaffActivityLogRepository, StaffActivityLogRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();



// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
// AI Chat Services (Groq)
builder.Services.AddHttpClient<IGroqService, GroqService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();

// Background Services
builder.Services.AddHostedService<CalendarReminderService>();

// SignalR Services
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();
builder.Services.AddScoped<IRealtimeNoteService, RealtimeNoteService>();
builder.Services.AddScoped<IRealtimeNotificationSender, RealtimeNotificationSenderAdapter>();



// Handlers
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddScoped<ForgotPasswordHandler>();
builder.Services.AddScoped<ChatHandler>();
builder.Services.AddScoped<StaffHandler>();
builder.Services.AddScoped<ContactHandler>();
builder.Services.AddScoped<LeadHandler>();
builder.Services.AddScoped<CustomerHandler>();
builder.Services.AddScoped<DealHandler>();
builder.Services.AddScoped<StatisticsHandler>();
builder.Services.AddScoped<ChartDealHandler>();
builder.Services.AddScoped<TaskHandler>();
builder.Services.AddScoped<NoteHandler>();
builder.Services.AddScoped<NotificationHandler>();
builder.Services.AddScoped<StaffPresenceHandler>();
builder.Services.AddScoped<TeamAssignmentHandler>();
builder.Services.AddScoped<AuditLogHandler>();
builder.Services.AddScoped<CalendarHandler>();
builder.Services.AddScoped<ReportHandler>();
builder.Services.AddScoped<ExportHandler>();

// Mapper Registration
builder.Services.AddAutoMapper(typeof(Program));

// HttpContextAccessor Registration
builder.Services.AddHttpContextAccessor();

// Redis Registration
builder.Services.AddStackExchangeRedisCache(options =>
{
    var host = builder.Configuration["RedisSettings:Host"];
    var port = builder.Configuration["RedisSettings:Port"];
    var password = builder.Configuration["RedisSettings:Password"];
    options.Configuration = $"{host}:{port},password={password},ssl=true,abortConnect=false";
});

// SendGrid and FluentEmail Registration
builder.Services
    .AddFluentEmail(builder.Configuration["SendGrid:Email"], builder.Configuration["SendGrid:Name"])
    .AddRazorRenderer()
    .AddSendGridSender(builder.Configuration["SendGrid:ApiKey"]);

// In-Memory Cache Registration
builder.Services.AddMemoryCache();


// GraphQL Registration
builder.Services
    .AddGraphQLServer()
    .AddType<QuantityStatisticsResponseType>()
    .AddType<DashboardResponseType>()
    .AddType<RevenueChartResponseType>()
    .AllowIntrospection(builder.Environment.IsDevelopment())
    .AddMaxExecutionDepthRule(8)
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<ChatQuery>()
        .AddTypeExtension<StaffQuery>()
        .AddTypeExtension<ContactQuery>()
        .AddTypeExtension<LeadQuery>()
        .AddTypeExtension<CustomerQuery>()
        .AddTypeExtension<DealQuery>()
        // .AddTypeExtension<StaffElasticSearchQuery>()
        // .AddTypeExtension<CustomersElasticSearchQuery>()
        // .AddTypeExtension<LeadsElasticSearchQuery>()
        // .AddTypeExtension<DealsElasticSearchQuery>()
        // .AddTypeExtension<ContactsElasticSearchQuery>()
        .AddTypeExtension<StatisticsQuery>()
        .AddTypeExtension<TaskQuery>()
        .AddTypeExtension<NoteQuery>()
        .AddTypeExtension<NotificationQuery>()
        .AddTypeExtension<StaffPresenceQuery>()
        .AddTypeExtension<TeamAssignmentQuery>()
        .AddTypeExtension<AuditLogQuery>()
        .AddTypeExtension<CalendarQuery>()
        .AddTypeExtension<ReportQuery>()

    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<AuthenticationMutation>()
        .AddTypeExtension<ForgotPasswordMutation>()
        .AddTypeExtension<ChatMutation>()
        .AddTypeExtension<StaffMutation>()
        .AddTypeExtension<LeadMutation>()
        .AddTypeExtension<ContactMutation>()
        .AddTypeExtension<CustomerMutation>()
        .AddTypeExtension<DealMutation>()
        .AddTypeExtension<TaskMutation>()
        .AddTypeExtension<NoteMutation>()
        .AddTypeExtension<NotificationMutation>()
        .AddTypeExtension<StaffPresenceMutation>()
        .AddTypeExtension<TeamAssignmentMutation>()
        .AddTypeExtension<CalendarMutation>()

    .AddType<DateType>()
    .AddErrorFilter<GraphQLExceptionFilter>()
    .AddFiltering()
    .AddAuthorization()
    .AddSorting()
    .AddProjections();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .WithHeaders("Content-Type", "Authorization", "Accept", "Origin", "X-Requested-With", "Apollo-Require-Preflight")
              .WithMethods("GET", "POST", "OPTIONS")
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// Authentication Config
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Rate Limiting — protects auth endpoints from brute force and OTP guessing
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login-attempts", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    options.AddPolicy("otp-attempts", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            });
    });

    options.AddPolicy("graphql-default", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});


var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseVoyager("/graphql", "/voyager");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint GraphQL
app.MapGraphQL("/graphql").WithOptions(new HotChocolate.AspNetCore.GraphQLServerOptions
{
    EnableSchemaRequests = builder.Environment.IsDevelopment(),
    EnableGetRequests = builder.Environment.IsDevelopment()
});

// SignalR Hubs
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<NoteHub>("/hubs/notes");

app.MapControllers();

// Health endpoints (no auth). Liveness = process alive; Readiness = DB+Redis reachable.
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                durationMs = e.Value.Duration.TotalMilliseconds
            })
        };
        await System.Text.Json.JsonSerializer.SerializeAsync(context.Response.Body, payload);
    }
});

app.Run();

