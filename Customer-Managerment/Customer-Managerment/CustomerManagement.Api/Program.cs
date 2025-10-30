using Customer_Managerment.CustomerManagement.Api.MiddleWare;
using Customer_Managerment.CustomerManagement.Api.Mutation;
using Customer_Managerment.CustomerManagement.Api.Query;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Application.UseCases.Authen;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Repositories;
using Customer_Managerment.CustomerManagement.Infrastructure.Services;
using HotChocolate.AspNetCore.Voyager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Configuration["Elasticsearch:Uri"] = Environment.GetEnvironmentVariable("ES__URL");
builder.Configuration["GeminiSettings:ApiKey"] = Environment.GetEnvironmentVariable("APIKEY__GEMINI");
builder.Configuration["GeminiSettings:BaseUrl"] = Environment.GetEnvironmentVariable("GEMINI__APIURL");


// DbContext Registration
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



// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();



// Handlers
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddScoped<ForgotPasswordHandler>();
builder.Services.AddScoped<ChatHandler>();
builder.Services.AddScoped<StaffHandler>();
builder.Services.AddScoped<ContactHandler>();
builder.Services.AddScoped<LeadHandler>();
builder.Services.AddScoped<CustomerHanler>();
builder.Services.AddScoped<DealHandler>();



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
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<ChatQuery>()
        .AddTypeExtension<StaffQuery>()
        .AddTypeExtension<ContactQuery>()
        .AddTypeExtension<LeadQuery>()
        .AddTypeExtension<CustomerQuery>()
        .AddTypeExtension<DealQuery>()

    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<RegisterMutation>()
        .AddTypeExtension<AuthenticationMutation>()
        .AddTypeExtension<ForgotPasswordMutation>()
        .AddTypeExtension<ChatMutation>()
        .AddTypeExtension<StaffMutation>()
        .AddTypeExtension<LeadMutation>()
        .AddTypeExtension<ContactMutation>()
        .AddTypeExtension<CustomerMutation>()
        .AddTypeExtension<DealMutation>()

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
        policy.WithOrigins("http://localhost:4200")
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

// Voyager UI để trực quan hóa schema
app.UseVoyager(
    "/graphql",  // endpoint của GraphQL
    "/voyager"   // đường dẫn để mở giao diện Voyager
);


app.MapControllers();

app.Run();

