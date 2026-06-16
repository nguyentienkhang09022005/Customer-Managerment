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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

DotNetEnv.Env.Load();

builder.Configuration["ConnectionStrings:PostgreSQLConnection"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__PostgreSQLConnection");
builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("APPSETTINGS__SECRETKEY");
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("APPSETTINGS__ISSUER");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("APPSETTINGS__AUDIENCE");
builder.Configuration["JwtSettings:AccessTokenExpirationMinutes"] = Environment.GetEnvironmentVariable("APPSETTINGS__ACCESSTOKENEXP");
builder.Configuration["JwtSettings:RefreshTokenExpirationDays"] = Environment.GetEnvironmentVariable("APPSETTINGS__REFRESHTOKENEXP");
builder.Configuration["RedisSettings:Host"] = Environment.GetEnvironmentVariable("REDISSETTINGS__HOST");
builder.Configuration["RedisSettings:Port"] = Environment.GetEnvironmentVariable("REDISSETTINGS__PORT");
builder.Configuration["RedisSettings:Password"] = Environment.GetEnvironmentVariable("REDISSETTINGS__PASSWORD");
builder.Configuration["SendGrid:ApiKey"] = Environment.GetEnvironmentVariable("SENDER_APIKEY");
builder.Configuration["SendGrid:Email"] = Environment.GetEnvironmentVariable("SENDER_EMAIL");
builder.Configuration["SendGrid:Name"] = Environment.GetEnvironmentVariable("SENDER_NAME");
// builder.Configuration["Elasticsearch:Uri"] = Environment.GetEnvironmentVariable("ES__URL");
builder.Configuration["GroqSettings:ApiKey"] = Environment.GetEnvironmentVariable("GROQ__APIKEY");
builder.Configuration["GroqSettings:BaseUrl"] = Environment.GetEnvironmentVariable("GROQ__APIURL");


// DbContext Registration
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPooledDbContextFactory<CustomerManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"))
);


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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://collaborative-model.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Allow send Cookie
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
    options.RequireHttpsMetadata = false;
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


var app = builder.Build();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint GraphQL
app.MapGraphQL("/graphql");

// SignalR Hubs
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<NoteHub>("/hubs/notes");

// Voyager UI
app.UseVoyager(
    "/graphql",
    "/voyager"
);


app.MapControllers();

app.Run();

